# SQL Design Document — Synthea Patient Dataset Import

**Author:** Dev SQL Agent (`patient-api-dev-sql`)
**Date:** 2026-07-14
**Status:** Draft — schema proposal only, nothing created yet
**Source:** `documents/patient_details/synthea/` — 17 CSV files. Read via header + one
sample row per file only (files are large; full content not loaded).

---

## Dataset identification
This is the standard **Synthea** synthetic patient data export format (a widely used
open-source synthetic EHR/claims data generator) — recognizable by its exact column
names (`BASE_ENCOUNTER_COST`, `SNOMED-CT` codes, `ENCOUNTERCLASS`, etc.) and its
UUID style (`xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`, e.g.
`fe621c76-a591-b7be-5668-b77f00240d82`).

## Central entity
**`patients`** is the hub — almost every other table has a `PATIENT`/`PATIENTID` FK
back to it. **`encounters`** is the second most-referenced table (a clinical visit;
most clinical-event tables hang off `PATIENT` + `ENCOUNTER` together).

## Important gap found in the source data
Several tables have **no `Id` column at all** in the CSV — they rely on a natural
composite key (e.g. `PATIENT` + `ENCOUNTER` + `CODE` + `START`/`DATE`). For a real
relational schema, these need a **surrogate `Id` (BIGINT IDENTITY / SERIAL)** added,
since composite natural keys across 3-4 columns are painful as FK targets elsewhere
and Synthea itself doesn't guarantee they're unique in all edge cases (e.g. a patient
could theoretically have two identical-timestamp entries). Flagged per table below.

---

## Proposed Tables

### 1. `patients` (from `patients.csv`)
| Column | Type | Notes |
|---|---|---|
| `Id` | `UNIQUEIDENTIFIER` (PK) | Native UUID from source — keep as-is, don't regenerate |
| `BirthDate` | `DATE` | |
| `DeathDate` | `DATE NULL` | |
| `Ssn`, `Drivers`, `Passport` | `VARCHAR(20)` | PII — flag for encryption/masking decision |
| `Prefix`, `First`, `Middle`, `Last`, `Suffix`, `Maiden` | `VARCHAR(100)` | |
| `Marital` | `VARCHAR(10)` | |
| `Race`, `Ethnicity`, `Gender` | `VARCHAR(30)` | |
| `Birthplace`, `Address`, `City`, `State`, `County`, `Fips`, `Zip` | `VARCHAR(150)`/`VARCHAR(10)` | |
| `Lat`, `Lon` | `DECIMAL(10,7)` | |
| `HealthcareExpenses`, `HealthcareCoverage` | `DECIMAL(12,2)` | |
| `Income` | `INT` | |

### 2. `organizations` (from `organizations.csv`)
| Column | Type | Notes |
|---|---|---|
| `Id` | `UNIQUEIDENTIFIER` (PK) | |
| `Name`, `Address`, `City`, `State`, `Zip`, `Phone` | `VARCHAR` | |
| `Lat`, `Lon` | `DECIMAL(10,7)` | |
| `Revenue` | `DECIMAL(14,2)` | |
| `Utilization` | `INT` | |

### 3. `providers` (from `providers.csv`)
| Column | Type | Notes |
|---|---|---|
| `Id` | `UNIQUEIDENTIFIER` (PK) | |
| `OrganizationId` | `UNIQUEIDENTIFIER` (FK → `organizations.Id`) | source column: `ORGANIZATION` |
| `Name`, `Gender`, `Speciality`, `Address`, `City`, `State`, `Zip` | `VARCHAR` | |
| `Lat`, `Lon` | `DECIMAL(10,7)` | |
| `Encounters`, `Procedures` | `INT` | denormalized counts from source, likely re-derivable via query instead of stored |

### 4. `payers` (from `payers.csv`)
| Column | Type | Notes |
|---|---|---|
| `Id` | `UNIQUEIDENTIFIER` (PK) | |
| `Name`, `Ownership`, `Address`, `City`, `StateHeadquartered`, `Zip`, `Phone` | `VARCHAR` | |
| `AmountCovered`, `AmountUncovered`, `Revenue` | `DECIMAL(14,2)` | |
| `CoveredEncounters`, `UncoveredEncounters`, `CoveredMedications`, `UncoveredMedications`, `CoveredProcedures`, `UncoveredProcedures`, `CoveredImmunizations`, `UncoveredImmunizations`, `UniqueCustomers`, `MemberMonths` | `INT` | |
| `QolsAvg` | `DECIMAL(6,4)` | source: `QOLS_AVG` |

### 5. `payer_transitions` (from `payer_transitions.csv`) — ⚠️ no `Id` in source
| Column | Type | Notes |
|---|---|---|
| `Id` | `BIGINT IDENTITY` (PK) | **surrogate key needed** |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `MemberId` | `UNIQUEIDENTIFIER` | |
| `StartDate`, `EndDate` | `DATETIME2 NULL` | |
| `PayerId` | `UNIQUEIDENTIFIER` (FK → `payers.Id`) | |
| `SecondaryPayerId` | `UNIQUEIDENTIFIER NULL` (FK → `payers.Id`) | |
| `PlanOwnership`, `OwnerName` | `VARCHAR` | |

### 6. `encounters` (from `encounters.csv`)
| Column | Type | Notes |
|---|---|---|
| `Id` | `UNIQUEIDENTIFIER` (PK) | |
| `Start`, `Stop` | `DATETIME2 NULL` | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `OrganizationId` | `UNIQUEIDENTIFIER` (FK → `organizations.Id`) | |
| `ProviderId` | `UNIQUEIDENTIFIER` (FK → `providers.Id`) | |
| `PayerId` | `UNIQUEIDENTIFIER NULL` (FK → `payers.Id`) | |
| `EncounterClass` | `VARCHAR(30)` | e.g. "wellness" |
| `Code`, `Description` | `VARCHAR` | clinical code (SNOMED) + description |
| `BaseEncounterCost`, `TotalClaimCost`, `PayerCoverage` | `DECIMAL(12,2)` | |
| `ReasonCode`, `ReasonDescription` | `VARCHAR NULL` | |

### 7. `conditions` (from `conditions.csv`) — ⚠️ no `Id` in source
| Column | Type | Notes |
|---|---|---|
| `Id` | `BIGINT IDENTITY` (PK) | **surrogate key needed** |
| `Start`, `Stop` | `DATE NULL` | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `EncounterId` | `UNIQUEIDENTIFIER` (FK → `encounters.Id`) | |
| `System` | `VARCHAR(60)` | e.g. "http://snomed.info/sct" |
| `Code`, `Description` | `VARCHAR` | |

### 8. `allergies` (from `allergies.csv`) — ⚠️ no `Id` in source
| Column | Type | Notes |
|---|---|---|
| `Id` | `BIGINT IDENTITY` (PK) | **surrogate key needed** |
| `Start`, `Stop` | `DATE NULL` | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `EncounterId` | `UNIQUEIDENTIFIER` (FK → `encounters.Id`) | |
| `Code`, `System`, `Description` | `VARCHAR` | |
| `Type`, `Category` | `VARCHAR(30)` | |
| `Reaction1`, `Description1`, `Severity1`, `Reaction2`, `Description2`, `Severity2` | `VARCHAR NULL` | up to 2 reactions per row — a **candidate for normalizing into a child `allergy_reactions` table** instead of repeated columns, worth a follow-up decision |

### 9. `medications` (from `medications.csv`) — ⚠️ no `Id` in source
| Column | Type | Notes |
|---|---|---|
| `Id` | `BIGINT IDENTITY` (PK) | **surrogate key needed** |
| `Start`, `Stop` | `DATETIME2 NULL` | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `PayerId` | `UNIQUEIDENTIFIER NULL` (FK → `payers.Id`) | |
| `EncounterId` | `UNIQUEIDENTIFIER` (FK → `encounters.Id`) | |
| `Code`, `Description` | `VARCHAR` | |
| `BaseCost`, `PayerCoverage`, `TotalCost` | `DECIMAL(12,2)` | |
| `Dispenses` | `INT` | |
| `ReasonCode`, `ReasonDescription` | `VARCHAR NULL` | |

### 10. `careplans` (from `careplans.csv`)
| Column | Type | Notes |
|---|---|---|
| `Id` | `UNIQUEIDENTIFIER` (PK) | |
| `Start`, `Stop` | `DATE NULL` | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `EncounterId` | `UNIQUEIDENTIFIER` (FK → `encounters.Id`) | |
| `Code`, `Description` | `VARCHAR` | |
| `ReasonCode`, `ReasonDescription` | `VARCHAR NULL` | |

### 11. `procedures` (from `procedures.csv`) — ⚠️ no `Id` in source
| Column | Type | Notes |
|---|---|---|
| `Id` | `BIGINT IDENTITY` (PK) | **surrogate key needed** |
| `Start`, `Stop` | `DATETIME2 NULL` | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `EncounterId` | `UNIQUEIDENTIFIER` (FK → `encounters.Id`) | |
| `System`, `Code`, `Description` | `VARCHAR` | |
| `BaseCost` | `DECIMAL(12,2)` | |
| `ReasonCode`, `ReasonDescription` | `VARCHAR NULL` | |

### 12. `immunizations` (from `immunizations.csv`) — ⚠️ no `Id` in source
| Column | Type | Notes |
|---|---|---|
| `Id` | `BIGINT IDENTITY` (PK) | **surrogate key needed** |
| `Date` | `DATETIME2` | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `EncounterId` | `UNIQUEIDENTIFIER` (FK → `encounters.Id`) | |
| `Code`, `Description` | `VARCHAR` | |
| `BaseCost` | `DECIMAL(12,2)` | |

### 13. `observations` (from `observations.csv`) — ⚠️ no `Id` in source
| Column | Type | Notes |
|---|---|---|
| `Id` | `BIGINT IDENTITY` (PK) | **surrogate key needed** |
| `Date` | `DATETIME2` | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `EncounterId` | `UNIQUEIDENTIFIER` (FK → `encounters.Id`) | |
| `Category` | `VARCHAR(30)` | e.g. "vital-signs" |
| `Code`, `Description` | `VARCHAR` | |
| `Value` | `VARCHAR(50)` | **mixed type in source** (numeric like "167.7" or text) — keep as string, or split into `NumericValue DECIMAL NULL` + `TextValue VARCHAR NULL` depending on `Type` column |
| `Units` | `VARCHAR(20) NULL` | |
| `Type` | `VARCHAR(20)` | "numeric" etc. — discriminates how to interpret `Value` |

### 14. `devices` (from `devices.csv`) — ⚠️ no `Id` in source
| Column | Type | Notes |
|---|---|---|
| `Id` | `BIGINT IDENTITY` (PK) | **surrogate key needed** |
| `Start`, `Stop` | `DATETIME2` | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `EncounterId` | `UNIQUEIDENTIFIER` (FK → `encounters.Id`) | |
| `Code`, `Description` | `VARCHAR` | |
| `Udi` | `VARCHAR(250)` | Unique Device Identifier — long barcode-style string |

### 15. `supplies` (from `supplies.csv`) — ⚠️ no `Id` in source
| Column | Type | Notes |
|---|---|---|
| `Id` | `BIGINT IDENTITY` (PK) | **surrogate key needed** |
| `Date` | `DATE` | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `EncounterId` | `UNIQUEIDENTIFIER` (FK → `encounters.Id`) | |
| `Code`, `Description` | `VARCHAR` | |
| `Quantity` | `INT` | |

### 16. `imaging_studies` (from `imaging_studies.csv`)
| Column | Type | Notes |
|---|---|---|
| `Id` | `UNIQUEIDENTIFIER` (PK) | |
| `Date` | `DATETIME2` | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `EncounterId` | `UNIQUEIDENTIFIER` (FK → `encounters.Id`) | |
| `SeriesUid`, `InstanceUid`, `SopCode`, `SopDescription`, `ProcedureCode` | `VARCHAR` | DICOM identifiers |
| `BodysiteCode`, `BodysiteDescription`, `ModalityCode`, `ModalityDescription` | `VARCHAR` | |

### 17. `claims` (from `claims.csv`)
| Column | Type | Notes |
|---|---|---|
| `Id` | `UNIQUEIDENTIFIER` (PK) | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `ProviderId` | `UNIQUEIDENTIFIER` (FK → `providers.Id`) | |
| `PrimaryPatientInsuranceId`, `SecondaryPatientInsuranceId` | `UNIQUEIDENTIFIER NULL` (FK → `payer_transitions.Id` once surrogate key exists) | |
| `DepartmentId`, `PatientDepartmentId` | `INT NULL` | |
| `Diagnosis1`...`Diagnosis8` | `VARCHAR(20) NULL` | 8 repeated columns — **candidate for a normalized `claim_diagnoses` child table** instead |
| `ReferringProviderId`, `SupervisingProviderId` | `UNIQUEIDENTIFIER NULL` (FK → `providers.Id`) | |
| `AppointmentId` | `UNIQUEIDENTIFIER NULL` | |
| `CurrentIllnessDate`, `ServiceDate` | `DATETIME2` | |
| `Status1`, `Status2`, `StatusP` | `VARCHAR(20)` | |
| `Outstanding1`, `Outstanding2`, `OutstandingP` | `DECIMAL(12,2)` | |
| `LastBilledDate1`, `LastBilledDate2`, `LastBilledDateP` | `DATETIME2 NULL` | |
| `HealthcareClaimTypeId1`, `HealthcareClaimTypeId2` | `INT` | |

### 18. `claims_transactions` (from `claims_transactions.csv`)
| Column | Type | Notes |
|---|---|---|
| `Id` | `UNIQUEIDENTIFIER` (PK) | |
| `ClaimId` | `UNIQUEIDENTIFIER` (FK → `claims.Id`) | |
| `ChargeId` | `INT` | |
| `PatientId` | `UNIQUEIDENTIFIER` (FK → `patients.Id`) | |
| `Type` | `VARCHAR(20)` | e.g. "CHARGE" |
| `Amount` | `DECIMAL(12,2)` | |
| `Method` | `VARCHAR(20) NULL` | |
| `FromDate`, `ToDate` | `DATETIME2` | |
| `PlaceOfService` | `UNIQUEIDENTIFIER NULL` | |
| `ProcedureCode` | `VARCHAR(20)` | |
| `Modifier1`, `Modifier2` | `VARCHAR(10) NULL` | |
| `DiagnosisRef1`...`DiagnosisRef4` | `INT NULL` | |
| `Units` | `INT` | |
| `DepartmentId` | `INT NULL` | |
| `Notes` | `VARCHAR(250) NULL` | |
| `UnitAmount` | `DECIMAL(12,2)` | |
| `TransferOutId`, `TransferType` | `VARCHAR NULL` | |
| `Payments`, `Adjustments`, `Transfers`, `Outstanding` | `DECIMAL(12,2)` | |
| `AppointmentId` | `UNIQUEIDENTIFIER NULL` | |
| `LineNote` | `VARCHAR NULL` | |
| `PatientInsuranceId` | `INT NULL` | |
| `FeeScheduleId` | `UNIQUEIDENTIFIER NULL` | |
| `ProviderId`, `SupervisingProviderId` | `UNIQUEIDENTIFIER` (FK → `providers.Id`) | |

---

## Open decisions before this becomes real schema
1. **Database engine** — still TBD (SQL Server / PostgreSQL / other). Types above are written SQL-Server-flavored (`UNIQUEIDENTIFIER`, `DATETIME2`) — trivially mappable to Postgres (`UUID`, `TIMESTAMP`) once decided.
2. **Surrogate keys** — 9 of 18 tables (`allergies`, `conditions`, `medications`, `procedures`, `immunizations`, `observations`, `devices`, `supplies`, `payer_transitions`) have no `Id` in the source CSV and need one added.
3. **PII handling** — `patients.Ssn`/`Drivers`/`Passport` are real PII fields (even in synthetic data, worth building the encryption/masking pattern now rather than retrofitting later).
4. **Repeated-column normalization** — `allergies` (2 reaction slots), `claims` (8 diagnosis slots) repeat columns rather than using child tables. Recommend normalizing into child tables (`allergy_reactions`, `claim_diagnoses`) rather than replicating the flat CSV shape as-is.
5. **Scope** — do we need all 18 tables for an initial version, or should the first sprint only cover a subset (e.g. `patients` + `encounters` + `conditions`)?

---
*Drafted by: Dev SQL Agent (`patient-api-dev-sql`) | Date: 2026-07-14 | Status: Awaiting review — nothing created yet*
