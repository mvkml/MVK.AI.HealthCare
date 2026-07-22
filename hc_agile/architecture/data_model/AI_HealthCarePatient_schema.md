# `AI_HealthCarePatient` — Table & Column Reference

Source of truth: `AI.HealthCare.Patient.EF.Migrations.PatientDbContextModelSnapshot` (EF Core
model snapshot). Table names are the real, pluralized DB names — not the singular entity class
names in `AI.HealthCare.Patient.EF.Entities`.

## Patient Identity

| Table | Columns |
|---|---|
| **Patients** | Id, First, Last, Middle, Prefix, Suffix, Maiden, BirthDate, DeathDate, Birthplace, Marital, Race, Ethnicity, Gender, Ssn, Drivers, Passport, Address, City, County, State, Zip, Fips, Lat, Lon, Income, HealthcareExpenses, HealthcareCoverage |

## Hospital Infrastructure

| Table | Columns |
|---|---|
| **Organizations** | Id, Name, Address, City, State, Zip, Lat, Lon, Phone, Revenue, Utilization |
| **Providers** | Id, Name, OrganizationId (FK→Organizations), Gender, Speciality, Address, City, State, Zip, Lat, Lon, Encounters, Procedures |

## Hospital / Clinical Care

| Table | Columns |
|---|---|
| **Encounters** | Id, PatientId (FK→Patients), OrganizationId (FK→Organizations), ProviderId (FK→Providers), PayerId (FK→Payers, nullable), Start, Stop, EncounterClass, Code, Description, ReasonCode, ReasonDescription, BaseEncounterCost, TotalClaimCost, PayerCoverage |
| **Conditions** | Id, PatientId (FK→Patients), EncounterId (FK→Encounters), Code, Description, Start, Stop, System |
| **Allergies** | Id, PatientId (FK→Patients), EncounterId (FK→Encounters), Code, System, Description, Type, Category, Start, Stop, Description1, Description2, Reaction1, Severity1, Reaction2, Severity2 |
| **Careplans** | Id, PatientId (FK→Patients), EncounterId (FK→Encounters), Code, Description, ReasonCode, ReasonDescription, Start, Stop |
| **Immunizations** | Id, PatientId (FK→Patients), EncounterId (FK→Encounters), Date, Code, Description, BaseCost |
| **Procedures** | Id, PatientId (FK→Patients), EncounterId (FK→Encounters), Code, Description, System, Start, Stop, ReasonCode, ReasonDescription, BaseCost |
| **Devices** | Id, PatientId (FK→Patients), EncounterId (FK→Encounters), Code, Description, Udi, Start, Stop |
| **Supplies** | Id, PatientId (FK→Patients), EncounterId (FK→Encounters), Date, Code, Description, Quantity |
| **ImagingStudies** | Id, PatientId (FK→Patients), EncounterId (FK→Encounters), Date, StudyId, ModalityCode, ModalityDescription, BodysiteCode, BodysiteDescription, InstanceUid, SeriesUid, SopCode, SopDescription, ProcedureCode |
| **Medications** | Id, PatientId (FK→Patients), EncounterId (FK→Encounters), PayerId (FK→Payers, nullable), Code, Description, Start, Stop, BaseCost, PayerCoverage, Dispenses, TotalCost, ReasonCode, ReasonDescription |
| **Observations** | Id, PatientId (FK→Patients), EncounterId (FK→Encounters, nullable), Date, Category, Code, Description, Value, Units, Type |

## Insurance / Payer

| Table | Columns |
|---|---|
| **Payers** | Id, Name, Address, City, StateHeadquartered, Zip, Phone, AmountCovered, AmountUncovered, Revenue, CoveredEncounters, UncoveredEncounters, CoveredMedications, UncoveredMedications, CoveredProcedures, UncoveredProcedures, CoveredImmunizations, UncoveredImmunizations, UniqueCustomers, QolsAvg, MemberMonths, Ownership |
| **PayerTransitions** | Id, PatientId (FK→Patients), MemberId, StartDate, EndDate, PayerId (FK→Payers), SecondaryPayerId (FK→Payers, nullable), PlanOwnership, OwnerName |

## Billing / Claims

| Table | Columns |
|---|---|
| **Claims** | Id, PatientId (FK→Patients), ProviderId (FK→Providers), ReferringProviderId (FK→Providers, nullable), SupervisingProviderId (FK→Providers, nullable), AppointmentId (FK→Encounters, nullable), PrimaryPatientInsuranceId (FK→Payers, nullable), SecondaryPatientInsuranceId (FK→Payers, nullable), DepartmentId, PatientDepartmentId, ServiceDate, CurrentIllnessDate, Status1, Status2, StatusP, Outstanding1, Outstanding2, OutstandingP, LastBilledDate1, LastBilledDate2, LastBilledDateP, HealthcareClaimTypeId1, HealthcareClaimTypeId2, Diagnosis1–8 |
| **ClaimTransactions** | Id, ClaimId (FK→Claims), ChargeId, PatientId (FK→Patients), ProviderId (FK→Providers), SupervisingProviderId (FK→Providers), AppointmentId (FK→Encounters, nullable), PatientInsuranceId (nullable), PlaceOfServiceId (FK→Organizations, nullable), Type, Amount, Method, FromDate, ToDate, ProcedureCode, DiagnosisRef1–4, Modifier1–2, UnitAmount, Units, Transfers, TransferOutId, TransferType, Payments, Adjustments, Outstanding, LineNote, Notes |

## Current query-access scope (important)

Only **5 of these 18 tables** are allow-listed for the live Doctor-persona query path today —
`Patients`, `Encounters`, `Conditions`, `Providers`, `Organizations` — per
`hc_data_source/hc_sql/seed/001_doctor_persona_whitelist_seed.sql`. Insurance/Payer and
Billing/Claims tables exist in the schema but are out of scope until a later persona phase (see
`healthcare_ai_assistant_mcp_ollama_design.md` §7).

## Source references

- EF model snapshot: `hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/AI.HealthCare.Patient.EF/Migrations/PatientDbContextModelSnapshot.cs`
- Whitelist (currently query-accessible subset): `hc_data_source/hc_sql/seed/001_doctor_persona_whitelist_seed.sql`
- Domain map / design intent: `hc_agile/architecture/design_patterns/healthcare_ai_assistant_mcp_ollama_design.md` §7
