# SOLID Principles — Owned by Architect Agent

Single holder file tracking which SOLID principles are applied where, across `AI.HealthCare.Patient.API`. Update this file (don't create one-per-principle) whenever a new instance is implemented — same convention `ai_hr`'s own `SOLID_PRINCIPLES.md` uses.

## S — Single Responsibility Principle
| Class | Responsibility |
|---|---|
| `<Domain>Repository` (all 18) | Only persistence (CRUD) for its own table, via `PatientDbContext` |
| `<Domain>Mapper` (`Patient` done, 2026-07-15) | Only Entity↔Item conversion — extracted out of `<Domain>Repository`, which previously did both persistence and mapping. See `design_patterns/MAPPER_PATTERN.md` |
| `PatientDbContext` | Only EF Core model configuration + change tracking — no business rules |
| `<Domain>Item` (Models, all 18) | Only carries data between layers — no behavior |
| `<Domain>BL` (not yet built) | Will only orchestrate business rules + validation for its domain — no persistence, no HTTP concerns |
| `<Domain>Controller` (not yet built) | Will only handle HTTP routing/status codes — delegates everything else to BL |

## O — Open/Closed Principle
- Not yet exercised — no extension points required so far (one concrete implementation per interface, no polymorphic strategy selection anywhere in this project yet).

## L — Liskov Substitution Principle
- Every `<Domain>Repository` is fully substitutable behind its `I<Domain>Repository` — no consumer needs to know the concrete type.

## I — Interface Segregation Principle
- Each of the 18 repository interfaces exposes only what that domain needs — standard 5-method CRUD, plus `GetByPatientId`/`GetByClaimId` only on the 13 domains where that query pattern is actually used. No single fat `IRepository<T>` covering all domains.
- **`<Domain>Mapper` (2026-07-15)**: one mapper interface per domain (`IPatientMapper`, etc.), not one shared `IMapUtility` covering all 18 — a single shared mapper was considered and rejected, since it would create one class with 18 unrelated reasons to change (unlike `CommonUtility`, which is genuinely domain-agnostic). Per-domain mappers mirror the same segregation already applied to Repositories.

## D — Dependency Inversion Principle
- All 18 `<Domain>Repository` classes depend on `PatientDbContext` via constructor injection (EF Core's DbContext is the persistence boundary, not abstracted further — same stance as `AI.HR.Api`).
- DI registrations in `Program.cs` map interface → concrete implementation (`AddScoped<IPatientRepository, PatientRepository>()`, etc.) for all 18 domains.
- See `design_patterns/DEPENDENCY_INJECTION.md` for full details.
- **Going forward**: `<Domain>BL` will depend on `I<Domain>Repository` (never the concrete class), and `<Domain>Controller` will depend on `I<Domain>BL` — continuing the same top-to-bottom interface-only dependency chain established in `AI.HR.Api`.

---

## Notes for the vertical build phase (BL + Controllers, one domain at a time)
Starting with `patients`. Every subsequent domain should repeat the same shape:
- `I<Domain>BL`/`<Domain>BL` — constructor-injects `I<Domain>Repository`, registered **Scoped**.
- `<Domain>Controller` — constructor-injects `I<Domain>BL`, full CRUD endpoints (GET by id, GET all, POST, PUT, DELETE).
- Update this file's tables above if a new SOLID-relevant decision comes up (e.g. an interface needing to grow, or a substitution concern) — don't wait until the whole vertical build is done.

---
*Defined by: Architect Agent | Date: 2026-07-15*
