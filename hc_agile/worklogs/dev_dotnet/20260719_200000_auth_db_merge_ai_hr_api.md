# Dev .NET Agent — Work Log
## Date: 2026-07-19
## Subject: Merge AI.HR.Api's database into AI_HealthCarePatient (PB023)

## Context
User walked through an existing, previously-untracked ASP.NET Core solution at
`hc_ai_in/mapi/AI.HR.Api` — full layered architecture (`AI.HR.Api`/`AI.HR.BL`/`AI.HR.DAL`/
`AI.HR.EF`/`AI.HR.Models`/`AI.HR.Repoistories`/`AI.HR.Common`) implementing signup, login,
forgot-password, reset-password, and roles lookup, with JWT issuing (`TokenService`) and password
hashing (`PasswordHasher`). It was running against its own `AI_HR` LocalDB database and was
invisible to the rest of the agile setup (not in git, not in `hc_agile/team/`, not in
`BACKLOG.md`). This unblocks PB023 ("Real auth backend"), which had been waiting on exactly this:
a DB schema and a working auth API.

## What Was Done
- Repointed `AI.HR.Api/appsettings.json`'s `AiHrDb` connection string from `Database=AI_HR` to
  `Database=AI_HealthCarePatient` — the same database `AI.HealthCare.Patient.API` already uses
- Confirmed `dotnet build AI.HR.Api.sln` succeeds unchanged
- Applied `AI.HR.EF`'s existing 4 migrations to `AI_HealthCarePatient` via
  `dotnet ef database update --project AI.HR.EF --startup-project AI.HR.Api` — additive only,
  verified via `INFORMATION_SCHEMA.TABLES` that all 23 pre-existing tables (Patients, Encounters,
  whitelist/audit tables, etc.) were untouched; 3 new tables added (`Users`, `Roles`,
  `OcrDocuments`)
- Copied the 9 existing rows from `AI_HR.dbo.Users` and 2 rows from `AI_HR.dbo.OcrDocuments` into
  `AI_HealthCarePatient` with `IDENTITY_INSERT` on, preserving IDs — `Roles` seed data is identical
  in both databases (same `HasData` seed in `AiHrDbContext`), so `RoleId` foreign keys carried over
  correctly with no remapping needed
- **`AI_HR` database itself was not modified in any way** — only read from, per explicit user
  instruction not to delete anything
- Live smoke test: ran `AI.HR.Api` on `localhost:5299` against the merged database, called
  `POST /api/users/signup` then `POST /api/users/login` with a throwaway test account — signup
  succeeded, login returned a valid JWT. Deleted the throwaway test row afterward; stopped the
  dev server
- Updated `hc_agile/team/dev_dotnet_agent.md` — added `hc_ai_in/mapi/AI.HR.Api/` to "Owns"
- Updated `BACKLOG.md` — PB023 status, added PB025 (Angular integration, tracked separately)
- Added `hc_agile/scrum/tasks/TASK013_US009_wire_real_auth_backend.md` for Dev Angular Agent

## Decisions Made
- Target database: `AI_HealthCarePatient` (the project's existing primary database), not a new
  shared DB and not keeping `AI_HR` as the login source of truth — per explicit user direction
- No data was deleted or moved destructively — `AI_HR` remains intact as a fallback/reference;
  the merge is a one-way additive copy into `AI_HealthCarePatient`

## Pending / Next Steps
- **Open item, not resolved here**: `AI.HR.Api`'s seeded roles are HR-domain (HR Manager,
  Recruiter, Payroll Manager, Team Lead, Developer, Other) — this project needs Doctor/Patient
  personas. Needs Product Owner/Architect sign-off before Angular signup can correctly map to a
  role. Flagged in TASK013, not guessed at here
- Token storage strategy (`localStorage` vs httpOnly cookie) for the real frontend integration —
  still open, was already flagged as unresolved in the original PB023 note
- Dev Angular Agent to pick up TASK013 (PB025) — wiring the mock auth UI to these real endpoints
- Consider whether `AI.HR.Api` should be added to git (currently untracked) — not done in this
  session, no instruction to commit was given

## References
- [BACKLOG.md](../../product_owner/backlog/BACKLOG.md) — PB023, PB025
- [TASK013](../../scrum/tasks/TASK013_US009_wire_real_auth_backend.md)
- [hc_data_source/hc_sql worklog](../dev_sql/20260719_200500_auth_tables_merge.md) — Dev SQL
  Agent's side of this same session
