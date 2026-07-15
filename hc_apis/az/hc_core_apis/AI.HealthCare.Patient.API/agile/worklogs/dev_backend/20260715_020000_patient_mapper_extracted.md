# 💻 Dev Backend Agent — Work Log
## Date: 2026-07-15
## Time: 02:00:00
## Subject: PatientMapper extracted from PatientRepository (SRP refactor)

### What Was Done
- Created `IPatientMapper`/`PatientMapper` in `AI.HealthCare.Patient.Repositories` — `ToModel(EfPatient)`/`ToEntity(PatientItem)`, moved verbatim out of `PatientRepository`'s former private static methods.
- `PatientRepository` now constructor-injects `IPatientMapper` alongside `PatientDbContext`; no mapping logic remains inside the Repository class.
- Registered `IPatientMapper`/`PatientMapper` as **Scoped** in `Program.cs`.
- Solution builds clean.
- **Verified no behavior change**: re-ran the full curl smoke test (Create with `includePii=true`, GetById, GetAll, Delete) — identical output to before the refactor. Re-ran `PatientValidationServiceTests` — 7/7 still pass.
- Updated `SOLID_PRINCIPLES.md` (SRP + ISP sections) and created `design_patterns/MAPPER_PATTERN.md`, documenting the decision to use **one mapper per domain** rather than a single shared `IMapUtility` — a shared mapper was proposed and explicitly rejected (would create one class with 18 unrelated reasons to change, the opposite of SRP's intent).

### Decisions Made
- Per-domain mapper pattern (`I<Domain>Mapper`/`<Domain>Mapper`), Scoped lifetime, one per domain — mirrors the existing per-domain Repository pattern.
- Scope of this pass: `Patient` only, as a proof of concept.

### Pending / Next Steps
- Rollout to the other 17 domains not yet decided: horizontal pass (mechanical, low-risk, could do all at once) vs. one-by-one alongside each domain's future BL/Controller work — awaiting user direction.
- No functional change to any endpoint — this was a pure internal refactor.
