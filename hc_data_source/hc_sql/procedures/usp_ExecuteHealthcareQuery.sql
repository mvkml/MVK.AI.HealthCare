-- usp_ExecuteHealthcareQuery
-- PB013: dynamic data access for the Doctor Chat prompt (US007).
--
-- Design intent: the LLM (Tool Layer) never supplies raw SQL text. It supplies a root table,
-- an optional single join table, a select/filter/order shape, and a max-records ceiling -- the
-- same query DSL already modeled in HC.AI.MAPI.Models (QueryRequest/QueryFilter/QueryOrderBy),
-- extended here with one join. Every table, column, and join identifier is resolved against
-- TableWhitelist/ColumnWhitelist/JoinWhitelist before it is allowed into the dynamic SQL text
-- (via QUOTENAME); every filter VALUE is bound as an sp_executesql parameter, never concatenated.
-- This keeps "dynamic query, unknown shape until runtime" while staying inside the "structured
-- DSL, not raw SQL" decision locked in healthcare_ai_assistant_mcp_ollama_design.md #4.

CREATE OR ALTER PROCEDURE dbo.usp_ExecuteHealthcareQuery
    @Persona          NVARCHAR(50),
    @RootTable        NVARCHAR(128),
    @JoinTable        NVARCHAR(128)    = NULL,
    @SelectColumnsCsv NVARCHAR(MAX)    = NULL,
    @FiltersJson      NVARCHAR(MAX)    = NULL,  -- '[{"column":"Last","op":"eq","value":"Smith"}]', max 5 entries
    @OrderByColumn    NVARCHAR(128)    = NULL,
    @OrderByDirection NVARCHAR(4)      = 'ASC',
    @MaxRecords       INT              = 10,
    @ProviderId       UNIQUEIDENTIFIER = NULL,   -- scopes to the asking doctor's own patients when supplied
    @ExecutedBy       NVARCHAR(128)    = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------------------------
    -- 1. Clamp MaxRecords server-side. The caller's value is a suggestion,
    --    never trusted as-is -- default 10, hard ceiling 50.
    ----------------------------------------------------------------------
    DECLARE @RequestedMax INT = @MaxRecords;
    IF @MaxRecords IS NULL OR @MaxRecords < 1 SET @MaxRecords = 10;
    IF @MaxRecords > 50 SET @MaxRecords = 50;

    ----------------------------------------------------------------------
    -- 2. Root table must be an allow-listed root for this persona.
    ----------------------------------------------------------------------
    IF NOT EXISTS (
        SELECT 1 FROM dbo.TableWhitelist
        WHERE Persona = @Persona AND TableName = @RootTable AND IsRoot = 1)
    BEGIN
        RAISERROR('Table "%s" is not an allow-listed root table for persona "%s".', 16, 1, @RootTable, @Persona);
        RETURN;
    END

    ----------------------------------------------------------------------
    -- 3. Optional join: FK/PK columns come only from JoinWhitelist, never
    --    from the caller -- this is what makes a "dynamic join" safe.
    ----------------------------------------------------------------------
    DECLARE @JoinFromCol NVARCHAR(128), @JoinToCol NVARCHAR(128);
    IF @JoinTable IS NOT NULL
    BEGIN
        SELECT @JoinFromCol = FromColumn, @JoinToCol = ToColumn
        FROM dbo.JoinWhitelist
        WHERE Persona = @Persona AND FromTable = @RootTable AND ToTable = @JoinTable;

        IF @JoinFromCol IS NULL
        BEGIN
            RAISERROR('Join %s -> %s is not allow-listed for persona "%s".', 16, 1, @RootTable, @JoinTable, @Persona);
            RETURN;
        END
    END

    ----------------------------------------------------------------------
    -- 4. Resolve SELECT columns against the whitelist; fall back to every
    --    selectable column on the root table if none/none valid were given.
    ----------------------------------------------------------------------
    DECLARE @SelectCols TABLE (ColumnName NVARCHAR(128));
    IF @SelectColumnsCsv IS NOT NULL
        INSERT INTO @SelectCols (ColumnName)
        SELECT cw.ColumnName
        FROM STRING_SPLIT(@SelectColumnsCsv, ',') s
        JOIN dbo.ColumnWhitelist cw
          ON cw.TableName = @RootTable AND cw.ColumnName = TRIM(s.value) AND cw.IsSelectable = 1;

    IF NOT EXISTS (SELECT 1 FROM @SelectCols)
        INSERT INTO @SelectCols (ColumnName)
        SELECT ColumnName FROM dbo.ColumnWhitelist
        WHERE TableName = @RootTable AND IsSelectable = 1;

    DECLARE @SelectList NVARCHAR(MAX) =
        (SELECT STRING_AGG(CONCAT(QUOTENAME(@RootTable), '.', QUOTENAME(ColumnName)), ', ')
         FROM @SelectCols);

    ----------------------------------------------------------------------
    -- 5. FROM / JOIN clause -- identifiers here only ever came from the
    --    whitelist lookups above, passed through QUOTENAME.
    ----------------------------------------------------------------------
    DECLARE @FromClause NVARCHAR(MAX) = QUOTENAME(@RootTable);
    IF @JoinTable IS NOT NULL
        SET @FromClause = CONCAT(@FromClause,
            ' INNER JOIN ', QUOTENAME(@JoinTable),
            ' ON ', QUOTENAME(@RootTable), '.', QUOTENAME(@JoinFromCol),
            ' = ', QUOTENAME(@JoinTable), '.', QUOTENAME(@JoinToCol));

    ----------------------------------------------------------------------
    -- 6. Filters (max 5). Column/op validated against the whitelist and a
    --    fixed operator allow-list; VALUES always bound as sp_executesql
    --    parameters -- never concatenated into the query text.
    ----------------------------------------------------------------------
    DECLARE @Filters TABLE (Slot INT, ColumnName NVARCHAR(128), Op NVARCHAR(20), FilterValue NVARCHAR(MAX));
    IF @FiltersJson IS NOT NULL
        INSERT INTO @Filters (Slot, ColumnName, Op, FilterValue)
        SELECT TOP (5) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1, f.[column], f.op, f.[value]
        FROM OPENJSON(@FiltersJson)
            WITH (
                [column] NVARCHAR(128) '$.column',
                op       NVARCHAR(20)  '$.op',
                [value]  NVARCHAR(MAX) '$.value'
            ) f
        JOIN dbo.ColumnWhitelist cw
            ON cw.TableName = @RootTable AND cw.ColumnName = f.[column] AND cw.IsFilterable = 1
        WHERE f.op IN ('eq', 'contains', 'gt', 'lt');

    DECLARE @WhereClause NVARCHAR(MAX) = N'1 = 1';
    DECLARE @f0 NVARCHAR(MAX), @f1 NVARCHAR(MAX), @f2 NVARCHAR(MAX), @f3 NVARCHAR(MAX), @f4 NVARCHAR(MAX);

    -- Ordered local-variable concatenation (deterministic here: single SELECT, no GROUP BY).
    SELECT @WhereClause = @WhereClause + CONCAT(' AND ', QUOTENAME(@RootTable), '.', QUOTENAME(ColumnName),
            CASE Op WHEN 'eq' THEN ' = ' WHEN 'gt' THEN ' > ' WHEN 'lt' THEN ' < ' WHEN 'contains' THEN ' LIKE ' END,
            CASE WHEN Op = 'contains' THEN CONCAT('''%'' + @f', Slot, ' + ''%''') ELSE CONCAT('@f', Slot) END)
    FROM @Filters
    ORDER BY Slot;

    SELECT
        @f0 = MAX(CASE WHEN Slot = 0 THEN FilterValue END),
        @f1 = MAX(CASE WHEN Slot = 1 THEN FilterValue END),
        @f2 = MAX(CASE WHEN Slot = 2 THEN FilterValue END),
        @f3 = MAX(CASE WHEN Slot = 3 THEN FilterValue END),
        @f4 = MAX(CASE WHEN Slot = 4 THEN FilterValue END)
    FROM @Filters;

    ----------------------------------------------------------------------
    -- 7. Optional ProviderId scoping -- when supplied, restricts results to
    --    the asking doctor's own patients (resolves the open scoping
    --    question in healthcare_ai_assistant_mcp_ollama_design.md #6/#10).
    ----------------------------------------------------------------------
    IF @ProviderId IS NOT NULL AND EXISTS (
        SELECT 1 FROM dbo.ColumnWhitelist WHERE TableName = @RootTable AND ColumnName = 'ProviderId')
        SET @WhereClause = @WhereClause + CONCAT(' AND ', QUOTENAME(@RootTable), '.', QUOTENAME('ProviderId'), ' = @ProviderIdParam');
    ELSE IF @ProviderId IS NOT NULL AND @JoinTable = 'Providers'
        SET @WhereClause = @WhereClause + CONCAT(' AND ', QUOTENAME(@JoinTable), '.', QUOTENAME('Id'), ' = @ProviderIdParam');

    ----------------------------------------------------------------------
    -- 8. Order by -- validated against the whitelist; no-op if absent/invalid.
    ----------------------------------------------------------------------
    DECLARE @OrderClause NVARCHAR(MAX) = N'';
    IF @OrderByColumn IS NOT NULL AND EXISTS (
        SELECT 1 FROM dbo.ColumnWhitelist WHERE TableName = @RootTable AND ColumnName = @OrderByColumn AND IsSortable = 1)
        SET @OrderClause = CONCAT(' ORDER BY ', QUOTENAME(@RootTable), '.', QUOTENAME(@OrderByColumn), ' ',
            CASE WHEN UPPER(@OrderByDirection) = 'DESC' THEN 'DESC' ELSE 'ASC' END);

    ----------------------------------------------------------------------
    -- 9. Assemble + execute. TOP (@TopN) and every filter/provider value
    --    are bound sp_executesql parameters -- the only text concatenation
    --    above is table/column identifiers sourced from the whitelist.
    ----------------------------------------------------------------------
    DECLARE @Sql NVARCHAR(MAX) = CONCAT(
        N'SELECT TOP (@TopN) ', @SelectList,
        N' FROM ', @FromClause,
        N' WHERE ', @WhereClause,
        @OrderClause);

    EXEC sp_executesql
        @Sql,
        N'@TopN INT, @f0 NVARCHAR(MAX), @f1 NVARCHAR(MAX), @f2 NVARCHAR(MAX), @f3 NVARCHAR(MAX), @f4 NVARCHAR(MAX), @ProviderIdParam UNIQUEIDENTIFIER',
        @TopN = @MaxRecords, @f0 = @f0, @f1 = @f1, @f2 = @f2, @f3 = @f3, @f4 = @f4, @ProviderIdParam = @ProviderId;

    ----------------------------------------------------------------------
    -- 10. Audit every execution -- required traceability for a healthcare
    --     data system driven by an LLM-constructed query.
    ----------------------------------------------------------------------
    INSERT INTO dbo.QueryAuditLog
        (Persona, RootTable, JoinTable, FiltersJson, MaxRecordsRequested, MaxRecordsApplied, ProviderId, RowsReturned, ExecutedBy)
    VALUES
        (@Persona, @RootTable, @JoinTable, @FiltersJson, @RequestedMax, @MaxRecords, @ProviderId, @@ROWCOUNT, @ExecutedBy);
END
GO
