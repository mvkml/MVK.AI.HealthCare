# Design Pattern — Repository

## Owned by
Architect Agent

## Where Implemented
`AI.HealthCare.Patient.Repositories/<Domain>/I<Domain>Repository.cs` + `<Domain>Repository.cs`, one subfolder per domain, for all 18 domains (`Patient`, `Organization`, `Provider`, `Payer`, `PayerTransition`, `Encounter`, `Condition`, `Allergy`, `Medication`, `Careplan`, `Procedure`, `Immunization`, `Observation`, `Device`, `Supply`, `ImagingStudy`, `Claim`, `ClaimTransaction`).

**Folder structure (2026-07-15)**: originally flat (36+ files in one folder), reorganized into one subfolder per domain — matches `AI.HealthCare.Patient.Models`'s existing per-domain subfolder convention. The namespace stays `AI.HealthCare.Patient.Repositories` for every file regardless of subfolder (C# doesn't require folder structure to match namespace, and all 18 domains' type names are already unique) — this was a pure file-tree reorganization with zero `using` statement changes anywhere in the solution.

## What It Is
Abstracts "how data is persisted" behind an interface, so nothing above the Repository layer (BL, Controllers) needs to know EF Core exists.

## How It Was Implemented
- One interface + one implementation per domain — e.g. `IPatientRepository`/`PatientRepository` — never one giant `IRepository<T>` generic covering all 18. Each interface exposes only what that domain needs: standard `GetById`/`GetAll`/`Create`/`Update`/`Delete`, plus a `GetByPatientId` (or `GetByClaimId` for `ClaimTransactions`) on the 13 domains that hang directly off `patients`/`claims`.
- Each `<Domain>Repository` takes `PatientDbContext` via constructor injection, converts between the EF entity and the `<Domain>Item` model (see `DTO_CARRIER_PATTERN.md`) — the EF entity type never crosses the Repository boundary.
- Entity/Item conversion is done with private static `ToModel`/`ToEntity` methods — no AutoMapper or reflection-based mapping, kept explicit and simple given the low field-count-to-complexity ratio.
- All repositories registered as **Scoped** in `Program.cs`, matching `PatientDbContext`'s own Scoped lifetime (avoids captive-dependency issues).

## Why
- Business logic (once BL exists) can be unit-tested against a mocked `I<Domain>Repository`, without a real database.
- Per-domain interfaces keep each one small and focused (Interface Segregation) — a consumer of `IPatientRepository` is never forced to depend on methods it doesn't call.
- Matches the same Repository pattern `ai_hr`'s `AI.HR.Api` already uses (see `AI.HR.Api`'s own `REPOSITORY_PATTERN.md`) — same architectural vocabulary, different domain.

---
*Defined by: Architect Agent | Date: 2026-07-15*
