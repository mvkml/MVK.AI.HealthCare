# Design Pattern — Layered (N-Tier) Architecture

## Owned by
Architect Agent

## Where Implemented
Whole solution: `AI.HealthCare.Patient.API` (Controllers) → `AI.HealthCare.Patient.BL` (not yet created) → `AI.HealthCare.Patient.Repositories` → `AI.HealthCare.Patient.EF` (DbContext/Entities) → `AI.HealthCare.Patient.Models` (DTOs, referenced by every layer above EF)

## What It Is
Each layer only talks to the layer directly below it, through an interface. A Controller never touches EF Core directly; BL never touches HTTP concerns; a Repository never contains business rules.

## How It Is Being Implemented
- **Controllers** (`AI.HealthCare.Patient.API/Controllers/`) — HTTP routing/status codes only. Depend on `I<Domain>BL`.
- **BL** (`AI.HealthCare.Patient.BL/`, next layer to build) — business rules, validation. Depends on `I<Domain>Repository`.
- **Repositories** (`AI.HealthCare.Patient.Repositories/`) — persistence only (CRUD against `PatientDbContext`). Done for all 18 domains.
- **EF** (`AI.HealthCare.Patient.EF/`) — `DBContexts/`, `Entities/`, `Migrations/`. Done for all 18 domains.
- **Models** (`AI.HealthCare.Patient.Models/`) — one subfolder + namespace per domain, `<Domain>Item` carrier class. Repositories/BL/Controllers pass this around; EF entities never leak above the Repository layer.

## Why
- Each layer is independently testable (mock the interface one level down).
- A change in persistence technology (e.g. swapping SQL Server for Postgres) only touches EF + Repositories, not BL or Controllers.
- Matches the same layering already proven out in `AI.HR.Api` (`ai_hr`'s own backend) — one architectural vocabulary across both projects, even though the domains are unrelated.

## Build order (this project)
Built horizontally (layer-by-layer, all 18 domains at once) for Models → EF → Repositories, since those layers had no per-domain business decisions to make. From BL onward, the team is switching to **vertical builds** — one domain gets its full BL + Controller before moving to the next — since BL/Controller work involves business-rule decisions that are easier to reason about one domain at a time rather than in a horizontal sweep.

---
*Defined by: Architect Agent | Date: 2026-07-15*
