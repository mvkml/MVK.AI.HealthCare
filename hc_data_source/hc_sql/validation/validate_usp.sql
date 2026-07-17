-- Companion script to usp_ExecuteHealthcareQuery_validation.md.
-- Run with: sqlcmd -S "(localdb)\MSSQLLocalDB" -d AI_HealthCarePatient -i validate_usp.sql -b -W

PRINT '--- TEST 1: single-patient lookup by exact last name ---';
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor',
    @RootTable = 'Patients',
    @FiltersJson = '[{"column":"Last","op":"eq","value":"Altenwerth646"}]',
    @MaxRecords = 10;
GO

PRINT '--- TEST 2: contains filter (partial last name) ---';
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor',
    @RootTable = 'Patients',
    @FiltersJson = '[{"column":"Last","op":"contains","value":"Alten"}]',
    @MaxRecords = 5;
GO

PRINT '--- TEST 3: Encounters joined to Patients, default MaxRecords (no value passed -> should default to 10) ---';
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor',
    @RootTable = 'Encounters',
    @JoinTable = 'Patients',
    @SelectColumnsCsv = 'Id,Start,Description,ReasonDescription',
    @OrderByColumn = 'Start',
    @OrderByDirection = 'DESC';
GO

PRINT '--- TEST 4: MaxRecords over the hard ceiling (100 requested -> should clamp to 50) ---';
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor',
    @RootTable = 'Encounters',
    @MaxRecords = 100;
GO

PRINT '--- TEST 5: invalid root table (not allow-listed) -> should raise an error, no rows ---';
BEGIN TRY
    EXEC dbo.usp_ExecuteHealthcareQuery
        @Persona = 'Doctor',
        @RootTable = 'Claims',
        @MaxRecords = 10;
END TRY
BEGIN CATCH
    PRINT 'Caught expected error: ' + ERROR_MESSAGE();
END CATCH
GO

PRINT '--- TEST 6: invalid join (not allow-listed) -> should raise an error, no rows ---';
BEGIN TRY
    EXEC dbo.usp_ExecuteHealthcareQuery
        @Persona = 'Doctor',
        @RootTable = 'Patients',
        @JoinTable = 'Claims',
        @MaxRecords = 10;
END TRY
BEGIN CATCH
    PRINT 'Caught expected error: ' + ERROR_MESSAGE();
END CATCH
GO

PRINT '--- TEST 7: ProviderId scoping via Encounters join to Providers ---';
DECLARE @SampleProviderId UNIQUEIDENTIFIER = (SELECT TOP 1 ProviderId FROM Encounters);
PRINT 'Using ProviderId = ' + CONVERT(NVARCHAR(50), @SampleProviderId);
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor',
    @RootTable = 'Encounters',
    @SelectColumnsCsv = 'Id,PatientId,ProviderId,Description',
    @ProviderId = @SampleProviderId,
    @MaxRecords = 5;
GO

PRINT '--- TEST 8: audit log rows recorded for the above runs ---';
SELECT Persona, RootTable, JoinTable, MaxRecordsRequested, MaxRecordsApplied, RowsReturned, ExecutedAtUtc
FROM dbo.QueryAuditLog
ORDER BY Id;
GO
