# Design Pattern — Dependency Injection (DI)

## Owned by
Architect Agent

## Where Implemented
`AI.HealthCare.Patient.API/Program.cs` (registration), all 18 `<Domain>Repository` classes, `PatientDbContext`.

## What It Is
Each class declares what it needs via its constructor instead of creating dependencies itself. ASP.NET Core's built-in container resolves and supplies them.

## How It Was Implemented
- `PatientDbContext` registered via `AddDbContext<PatientDbContext>(...)` — default Scoped lifetime.
- All 18 `<Domain>Repository` classes registered as **Scoped** via `AddScoped<I<Domain>Repository, <Domain>Repository>()` — matches `PatientDbContext`'s lifetime, avoiding captive-dependency issues (a longer-lived service holding a reference to a shorter-lived one).
- Every repository constructor-injects `PatientDbContext` directly (not an abstraction over it — EF Core's own DbContext is treated as the persistence boundary, same as `AI.HR.Api`'s convention).
- **Going forward (BL layer)**: `<Domain>BL` classes will constructor-inject `I<Domain>Repository` (the interface, never the concrete `<Domain>Repository`), and Controllers will constructor-inject `I<Domain>BL` — continuing the interface-only dependency chain top to bottom.
- Expected lifetime for BL: **Scoped**, for the same consistency reason `AI.HR.Api` chose Scoped for `PasswordHasher`/`CommonUtility` in that project — not a performance argument, a consistency one.

## Why
- Removes tight coupling and manual object construction from every layer.
- Lets the DI container manage object lifetimes consistently with `PatientDbContext`.
- Every layer depends on abstractions (interfaces), not concretions — enables mocking `I<Domain>Repository` in future BL unit tests without a real database.

---
*Defined by: Architect Agent | Date: 2026-07-15*
