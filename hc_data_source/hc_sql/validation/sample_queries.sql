-- Additional ready-to-run sample calls to dbo.usp_ExecuteHealthcareQuery, beyond the core
-- acceptance tests in validate_usp.sql. See sample_queries_results.md for actual output.
-- Run with: sqlcmd -S "(localdb)\MSSQLLocalDB" -d AI_HealthCarePatient -i sample_queries.sql -b -W

PRINT '--- SAMPLE 1: multiple filters combined (Gender=F AND State=Massachusetts) ---';
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor', @RootTable = 'Patients',
    @FiltersJson = '[{"column":"Gender","op":"eq","value":"F"},{"column":"State","op":"eq","value":"Massachusetts"}]',
    @MaxRecords = 5;
GO

PRINT '--- SAMPLE 2: gt filter on a date column (Encounters starting after 2026-01-01) ---';
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor', @RootTable = 'Encounters',
    @FiltersJson = '[{"column":"Start","op":"gt","value":"2026-01-01"}]',
    @OrderByColumn = 'Start', @OrderByDirection = 'ASC',
    @MaxRecords = 5;
GO

PRINT '--- SAMPLE 3: no filters at all, just root table + default select + default MaxRecords ---';
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor', @RootTable = 'Patients';
GO

PRINT '--- SAMPLE 4: filter that matches nothing -> should return 0 rows, not an error ---';
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor', @RootTable = 'Patients',
    @FiltersJson = '[{"column":"Last","op":"eq","value":"NoSuchPatientXYZ"}]',
    @MaxRecords = 5;
GO

PRINT '--- SAMPLE 5: Encounters joined to Conditions (reverse-FK join: Encounters.Id = Conditions.EncounterId) ---';
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor', @RootTable = 'Encounters', @JoinTable = 'Conditions',
    @SelectColumnsCsv = 'Id,Description',
    @MaxRecords = 5;
GO

PRINT '--- SAMPLE 6: MaxRecords = 0 -> should fall back to default (10), not 0 rows ---';
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor', @RootTable = 'Patients', @MaxRecords = 0;
GO

PRINT '--- SAMPLE 7: unknown persona -> root table not whitelisted for that persona -> rejected ---';
BEGIN TRY
    EXEC dbo.usp_ExecuteHealthcareQuery
        @Persona = 'Nurse', @RootTable = 'Patients', @MaxRecords = 5;
END TRY
BEGIN CATCH
    PRINT 'Caught expected error: ' + ERROR_MESSAGE();
END CATCH
GO

PRINT '--- SAMPLE 8: select column not on the whitelist (Ssn) -> silently dropped, not returned ---';
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor', @RootTable = 'Patients',
    @SelectColumnsCsv = 'First,Last,Ssn',
    @MaxRecords = 3;
GO
