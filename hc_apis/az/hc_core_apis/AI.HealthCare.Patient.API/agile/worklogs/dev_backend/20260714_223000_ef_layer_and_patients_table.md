# 💻 Dev Backend Agent — Work Log
## Date: 2026-07-14
## Time: 22:30:00
## Subject: Layer #2 — AI.HealthCare.Patient.EF scaffolded, patients table created

### What Was Done
- Created `AI.HealthCare.Patient.EF` class library, matching `AI.HR.EF`'s setup: `net8.0`, `Microsoft.EntityFrameworkCore.SqlServer`/`.Design`/`.Tools` 8.0.28, with `DBContexts/`, `Entities/`, `Migrations/` subfolders.
- Added `Entities/Patient.cs` — mapped from `patients.csv` per `SQL_DESIGN_synthea_import.md` (`Id` kept as native `UNIQUEIDENTIFIER`/`Guid`, not regenerated).
- Added `DBContexts/PatientDbContext.cs` with `DbSet<Patient> Patients`, column type/length constraints per the design doc (decimal(10,7) for Lat/Lon, decimal(12,2) for healthcare cost fields, PII fields as nvarchar(20)).
- DB engine decision (confirmed by user): SQL Server LocalDB, same instance as `AI_HR`/`AI_INS` (`(localdb)\MSSQLLocalDB`), new database `AI_HealthCarePatient`.
- Added `ConnectionStrings:PatientDb` to `appsettings.json`, registered `AddDbContext<PatientDbContext>` in `Program.cs`.
- Added EF Core Design package to the startup project (`AI.HealthCare.Patient.API`) — required by `dotnet ef` tooling, matching `AI.HR.Api`'s pattern.
- Ran `dotnet ef migrations add InitialCreate` and `dotnet ef database update` — verified: `AI_HealthCarePatient` database now exists on `(localdb)\MSSQLLocalDB` with a `Patients` table.

### Decisions Made
- Only the `patients` entity was created in this pass (matches the "start with `patients` first" plan, since it's the hub table). The other 17 tables are not yet scaffolded.
- `Id` uses `ValueGeneratedNever()` since Synthea's UUIDs are pre-existing values from the source CSV, not DB-generated.

### Pending / Next Steps
- Remaining 17 entities (organizations, providers, payers, payer_transitions, encounters, conditions, allergies, medications, careplans, procedures, immunizations, observations, devices, supplies, imaging_studies, claims, claims_transactions) not yet created — to be added incrementally.
- Layer #3 (`Repositories`) still needs the naming call (`Repositories` vs `Repoistories`) before it can start.
- CSV data import (loading the actual Synthea rows into `Patients`) not yet done — schema only so far.
