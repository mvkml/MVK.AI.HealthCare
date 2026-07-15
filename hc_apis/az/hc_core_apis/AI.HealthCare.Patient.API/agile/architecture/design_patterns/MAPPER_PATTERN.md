# Design Pattern — Domain Mapper

## Owned by
Architect Agent

## Where Implemented
`AI.HealthCare.Patient.Repositories/<Domain>/I<Domain>Mapper.cs` + `<Domain>Mapper.cs`, in the same per-domain subfolder as that domain's Repository — `Patient` done (2026-07-15); remaining 17 domains not yet extracted.

## What It Is
Converts between an EF entity (`AI.HealthCare.Patient.EF.Entities.<Domain>`) and its `<Domain>Item` model, in a class separate from the Repository. Previously, each `<Domain>Repository` had its own private `ToModel`/`ToEntity` static methods — mixing persistence (talking to `PatientDbContext`) with mapping (shaping data for callers) in one class.

## How It Was Implemented
- `IPatientMapper` — `PatientItem ToModel(EfPatient entity)`, `EfPatient ToEntity(PatientItem item)`.
- `PatientMapper` — the exact mapping logic that used to live as private static methods inside `PatientRepository`, moved out verbatim.
- `PatientRepository` constructor-injects `IPatientMapper`, calls `_mapper.ToModel(...)`/`_mapper.ToEntity(...)` — no mapping logic left inside the Repository class.
- Registered as **Scoped** in `Program.cs` (matches the Repository it's paired with — not Singleton, since there's no cross-domain sharing argument here unlike `CommonUtility`).

## Why one mapper per domain, not one shared `IMapUtility`
A single shared mapper covering all 18 domains' `ToModel`/`ToEntity` overloads was proposed and explicitly rejected during design: it would create one class with 18 unrelated reasons to change (a `Claims` mapping edit and a `Patients` mapping edit would both touch the same file), the opposite of what SRP is trying to achieve. `CommonUtility` (in `ai_hr`) works as a single shared class *because* its methods (`GenerateDcn`/`GenerateGrp`) are genuinely domain-agnostic string formatting — Entity↔Item mapping is inherently domain-specific and doesn't fit that shape. Per-domain mappers mirror the same segregation already used for Repositories (`I<Domain>Repository`, not one `IRepository<T>`).

## Rollout status
`Patient` only, as a proof of concept — verified via live smoke test (Create/GetById/GetAll/Delete via curl, all behaving identically to before the extraction) and the existing `PatientValidationServiceTests` (7/7 still passing). Whether to roll this out to the other 17 domains in one horizontal pass, or one-by-one alongside their future BL/Controller work, is still an open call.

---
*Defined by: Architect Agent | Date: 2026-07-15*
