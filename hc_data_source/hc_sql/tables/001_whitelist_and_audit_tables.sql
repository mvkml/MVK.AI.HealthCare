-- PB013/PB014 support tables: server-side allow-list metadata for the Doctor Chat prompt's
-- dynamic query stored procedure (usp_ExecuteHealthcareQuery). No table/column/join identifier
-- from the LLM-driven query DSL reaches dynamic SQL unless it round-trips through these tables.
-- This is the SQL-side enforcement of the Guardrail Layer described in US007.

CREATE TABLE dbo.TableWhitelist (
    Persona     NVARCHAR(50)  NOT NULL,
    TableName   NVARCHAR(128) NOT NULL,
    IsRoot      BIT           NOT NULL DEFAULT 0,  -- can this table be the query's FROM/root table
    CONSTRAINT PK_TableWhitelist PRIMARY KEY (Persona, TableName)
);
GO

CREATE TABLE dbo.ColumnWhitelist (
    TableName    NVARCHAR(128) NOT NULL,
    ColumnName   NVARCHAR(128) NOT NULL,
    IsSelectable BIT NOT NULL DEFAULT 1,
    IsFilterable BIT NOT NULL DEFAULT 1,
    IsSortable   BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_ColumnWhitelist PRIMARY KEY (TableName, ColumnName)
);
GO

CREATE TABLE dbo.JoinWhitelist (
    Persona     NVARCHAR(50)  NOT NULL,
    FromTable   NVARCHAR(128) NOT NULL,
    ToTable     NVARCHAR(128) NOT NULL,
    FromColumn  NVARCHAR(128) NOT NULL,  -- FK column on FromTable
    ToColumn    NVARCHAR(128) NOT NULL,  -- PK column on ToTable
    CONSTRAINT PK_JoinWhitelist PRIMARY KEY (Persona, FromTable, ToTable)
);
GO

-- Compliance/audit trail — every dynamic query the procedure runs, regardless of outcome.
CREATE TABLE dbo.QueryAuditLog (
    Id                  BIGINT IDENTITY(1,1) PRIMARY KEY,
    Persona             NVARCHAR(50)  NOT NULL,
    RootTable           NVARCHAR(128) NOT NULL,
    JoinTable           NVARCHAR(128) NULL,
    FiltersJson         NVARCHAR(MAX) NULL,
    MaxRecordsRequested INT NULL,
    MaxRecordsApplied   INT NOT NULL,
    ProviderId          UNIQUEIDENTIFIER NULL,
    RowsReturned        INT NOT NULL,
    ExecutedAtUtc       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ExecutedBy          NVARCHAR(128) NULL
);
GO
