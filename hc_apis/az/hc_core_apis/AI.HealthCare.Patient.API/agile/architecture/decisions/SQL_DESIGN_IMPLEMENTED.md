# SQL Design — As-Built (AI_HealthCarePatient)

## Status
**Implemented and live.** This document reflects the actual schema created in `AI_HealthCarePatient` on `(localdb)\MSSQLLocalDB`, via `AI.HealthCare.Patient.EF` (12 migrations, `InitialCreate` through `AddClaimTransactions`). It supersedes `SQL_DESIGN_synthea_import.md` wherever the two disagree — see **Deviations** below for what changed and why.

Source CSVs: `documents/patient_details/synthea/` (17 files, Synthea synthetic patient dataset). Companion data dictionary: `SYNTHEA_SCHEMA_REFERENCE.md`.

## Engine
SQL Server (LocalDB), matching the `AI_HR`/`AI_INS` pattern already in use on this machine. Connection string: `AI.HealthCare.Patient.API/appsettings.json` → `ConnectionStrings:PatientDb`.

## Deletion behavior
Every foreign key uses `DeleteBehavior.Restrict`. This was a blanket decision made partway through the build: SQL Server rejects a migration outright if the default `Cascade` creates more than one delete path to the same ancestor table (e.g. `encounters` → `organizations` directly, and also `encounters` → `providers` → `organizations`). Rather than case-by-case tuning, every FK in this schema restricts delete — deleting a `patient` (or any ancestor) requires deleting its dependents first.

## Deviations from the original proposal (`SQL_DESIGN_synthea_import.md`)
Found while building, based on what the actual CSV sample rows contained — not judgment calls:

1. **`claims.PrimaryPatientInsuranceId` / `SecondaryPatientInsuranceId`** — original proposal: FK → `payer_transitions.Id` (a `BIGINT` surrogate). Actual CSV values are GUIDs matching `payers.Id` (e.g. `df166300-5a78-3502-a46a-832842197811` appears identically as both `claims.PRIMARYPATIENTINSURANCEID` and `encounters.PAYER`). **Implemented as**: FK → `payers.Id`. The original target was type-incompatible with the real data.
2. **`claims.AppointmentId`** and **`claims_transactions.AppointmentId`** — original proposal: plain `UNIQUEIDENTIFIER NULL`, no FK. Sample data shows the value matches an `encounters.Id` for the same patient. **Implemented as**: nullable FK → `encounters.Id`.
3. **`claims_transactions.PlaceOfService`** — implemented as FK → `organizations.Id`, per the interpretation already recorded in `SYNTHEA_SCHEMA_REFERENCE.md`.

Everything else matches the original proposal's column list and types.

## Tables (as built)

### 1. `Patients`
PK `Id` (`GUID`, native, not regenerated). `BirthDate` (required), `DeathDate`, `Ssn`/`Drivers`/`Passport` (PII, `nvarchar(20)`), name parts, `Marital`/`Race`/`Ethnicity`/`Gender`, address fields, `Lat`/`Lon` (`decimal(10,7)`), `HealthcareExpenses`/`HealthcareCoverage` (`decimal(12,2)`), `Income`.

### 2. `Organizations`
PK `Id` (native GUID). `Name` (required), address fields, `Lat`/`Lon`, `Revenue` (`decimal(14,2)`), `Utilization`.

### 3. `Providers`
PK `Id` (native GUID). FK `OrganizationId` → `Organizations` (**Cascade** — the one exception; see note below). `Name` (required), `Gender`, `Speciality`, address, `Lat`/`Lon`, `Encounters`/`Procedures` (denormalized counts).

> Note: `Providers.OrganizationId` is the only FK left on the EF default (`Cascade`) — it predates the "always Restrict" rule adopted from `PayerTransitions` onward. Functionally harmless today (no second path existed yet when it was created), but inconsistent with the rest of the schema. Flagging for cleanup.

### 4. `Payers`
PK `Id` (native GUID). `Name` (required), `Ownership`, address, financial aggregates (`AmountCovered`/`AmountUncovered`/`Revenue` as `decimal(14,2)`), coverage counts, `QolsAvg` (`decimal(6,4)`), `MemberMonths`.

### 5. `PayerTransitions`
PK `Id` (`BIGINT IDENTITY` — surrogate, no natural key in source). FK `PatientId` → `Patients` (Restrict). FK `PayerId` → `Payers` (Restrict). FK `SecondaryPayerId` → `Payers`, nullable (Restrict). `MemberId`, `StartDate`/`EndDate`, `PlanOwnership`, `OwnerName`.

### 6. `Encounters`
PK `Id` (native GUID). FK `PatientId` → `Patients`, FK `OrganizationId` → `Organizations`, FK `ProviderId` → `Providers`, FK `PayerId` → `Payers` (nullable) — all Restrict. `Start`/`Stop`, `EncounterClass`, `Code`/`Description`, `BaseEncounterCost`/`TotalClaimCost`/`PayerCoverage` (`decimal(12,2)`), `ReasonCode`/`ReasonDescription`.

### 7. `Conditions`
PK `Id` (`BIGINT IDENTITY`). FK `PatientId`, FK `EncounterId` (both Restrict). `Start`/`Stop`, `System`, `Code`/`Description`.

### 8. `Allergies`
PK `Id` (`BIGINT IDENTITY`). FK `PatientId`, FK `EncounterId` (Restrict). `Type`, `Category`, `Code`/`System`/`Description`, two flat reaction slots (`Reaction1/Description1/Severity1`, `Reaction2/Description2/Severity2`) — **not normalized**; kept flat per the original proposal, still an open follow-up decision.

### 9. `Medications`
PK `Id` (`BIGINT IDENTITY`). FK `PatientId`, FK `PayerId` (nullable), FK `EncounterId` (Restrict). `Code`/`Description`, `BaseCost`/`PayerCoverage`/`TotalCost` (`decimal(12,2)`), `Dispenses`, `ReasonCode`/`ReasonDescription`.

### 10. `Careplans`
PK `Id` (native GUID). FK `PatientId`, FK `EncounterId` (Restrict). `Start`/`Stop`, `Code`/`Description`, `ReasonCode`/`ReasonDescription`.

### 11. `Procedures`
PK `Id` (`BIGINT IDENTITY`). FK `PatientId`, FK `EncounterId` (Restrict). `System`/`Code`/`Description`, `BaseCost` (`decimal(12,2)`), `ReasonCode`/`ReasonDescription`.

### 12. `Immunizations`
PK `Id` (`BIGINT IDENTITY`). FK `PatientId`, FK `EncounterId` (Restrict). `Date` (required), `Code`/`Description`, `BaseCost` (`decimal(12,2)`).

### 13. `Observations`
PK `Id` (`BIGINT IDENTITY`). FK `PatientId`, FK `EncounterId` (Restrict). `Date` (required), `Category`, `Code`/`Description`, `Value` (kept as `nvarchar(50)` — mixed numeric/text in source, not split), `Units`, `Type`.

### 14. `Devices`
PK `Id` (`BIGINT IDENTITY`). FK `PatientId`, FK `EncounterId` (Restrict). `Start`/`Stop`, `Code`/`Description`, `Udi` (`nvarchar(250)`).

### 15. `Supplies`
PK `Id` (`BIGINT IDENTITY`). FK `PatientId`, FK `EncounterId` (Restrict). `Date` (required), `Code`/`Description`, `Quantity`.

### 16. `ImagingStudies`
PK `Id` (native GUID). FK `PatientId`, FK `EncounterId` (Restrict). `Date` (required), DICOM identifiers (`SeriesUid`/`InstanceUid`/`SopCode`/`SopDescription`/`ProcedureCode`), `BodysiteCode`/`BodysiteDescription`, `ModalityCode`/`ModalityDescription`.

### 17. `Claims`
PK `Id` (native GUID). FK `PatientId`, FK `ProviderId` (Restrict). FK `PrimaryPatientInsuranceId`/`SecondaryPatientInsuranceId` → `Payers`, both nullable (Restrict) — **deviation, see above**. FK `ReferringProviderId`/`SupervisingProviderId` → `Providers`, both nullable (Restrict). FK `AppointmentId` → `Encounters`, nullable (Restrict) — **deviation, see above**. `DepartmentId`/`PatientDepartmentId` (plain `int`, no FK target confirmed). 8 flat diagnosis columns (`Diagnosis1`-`Diagnosis8`, `nvarchar(20)`) — not normalized, open follow-up decision. `CurrentIllnessDate`/`ServiceDate`, 3-way status/outstanding/last-billed-date triads (primary/secondary/patient), `HealthcareClaimTypeId1`/`2`.

### 18. `ClaimTransactions`
PK `Id` (native GUID). FK `ClaimId` → `Claims`, FK `PatientId` → `Patients`, FK `PlaceOfServiceId` → `Organizations` (nullable), FK `AppointmentId` → `Encounters` (nullable), FK `ProviderId`/`SupervisingProviderId` → `Providers` — all Restrict. `Type` (required), `Amount`/`UnitAmount`/`Payments`/`Adjustments`/`Transfers`/`Outstanding` (`decimal(12,2)`), `Method`, `FromDate`/`ToDate` (required), `ProcedureCode`, `Modifier1`/`Modifier2`, `DiagnosisRef1`-`4` (plain `int`, references a `Claims` diagnosis slot position — no FK, since diagnoses aren't normalized), `Units` (required), `DepartmentId`, `Notes`, `TransferOutId`/`TransferType`, `LineNote`, `PatientInsuranceId`, `FeeScheduleId` (plain, no confirmed FK target).

## Still open (unresolved, not yet acted on)
1. Normalize `Allergies` (2 reaction slots) and `Claims` (8 diagnosis slots) into child tables (`AllergyReactions`, `ClaimDiagnoses`) — flagged twice now, still undecided.
2. `Providers.OrganizationId` FK still on `Cascade` instead of `Restrict` — inconsistent with the rest of the schema, low-risk but worth a cleanup migration.
3. No data import yet — all 18 tables exist but are empty; the Synthea CSV rows haven't been loaded.
