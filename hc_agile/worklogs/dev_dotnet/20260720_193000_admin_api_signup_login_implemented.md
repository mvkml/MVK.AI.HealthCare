# Dev .NET Agent — Work Log
## Date: 2026-07-20
## Subject: Implement HC.AI.Admin.API signup + login (US016/US020, TASK016/TASK018, PB033)

## Context
User provided the endpoint scope for the previously-scaffolded (planning-only) `HC.AI.Admin.API`
story: sign up and login. Built the solution from scratch (no existing code to copy this time,
unlike `HC.AI.Identity.Api` which started from a pre-existing `AI.HR.Api`).

## What Was Done
- Scaffolded a new solution at `hc_apis/az/hc_core_apis/HC.AI.Admin.API/`: `HC.AI.Admin.Api`
  (webapi), `HC.AI.Admin.BL`, `HC.AI.Admin.EF`, `HC.AI.Admin.Models`, `HC.AI.Admin.Repositories`,
  `HC.AI.Admin.Api.Tests` — layered the same way as `HC.AI.Identity.Api`
- **Fixed two things during scaffold rather than carrying them forward:**
  - `dotnet new webapi` defaulted to `net9.0`; every other API in this repo targets `net8.0` —
    retargeted all 6 projects to `net8.0` for consistency
  - Hit a real C# compile error: naming the entity class `Admin` collided with the `HC.AI.Admin`
    namespace prefix (`CS0118: 'Admin' is a namespace but is used like a type`) — renamed the
    entity to `AdminAccount` (property/table name `Admins` unaffected)
- Built: `Admin` entity, `AdminDbContext` (EF), `IAdminRepository`/`AdminRepository`,
  `IAdminBL`/`AdminBL`, `IAdminValidationService`/`AdminValidationService`, `PasswordHasher`
  (PBKDF2-HMAC-SHA256, same algorithm as `HC.AI.Identity.Api`), `TokenService`/`JwtSettings` (JWT,
  `Issuer: HC.AI.Admin.Api`, `Audience: HC.AI.Admin.Web`, own random signing secret — not shared
  with Identity's), `AdminsController` (`POST /api/admins/signup`, `POST /api/admins/login`)
- **Deliberately did not copy** `Company`/`RoleId` fields from `Users` — those were HR-domain
  leftovers on the Identity side (flagged in earlier worklogs); `Admins` is `AdminId`, `FullName`,
  `Email`, `PasswordHash`, `CreatedAt`, `UpdatedAt`, `IsActive` only
- Added duplicate-email validation in `AdminValidationService.Validate` (async, checks
  `IAdminRepository.ExistsByEmail` before signup) — `HC.AI.Identity.Api`'s `UserBL.SignUp` uses
  `Upsert` instead and doesn't reject duplicates the same way; chose explicit rejection here since
  "sign up" for a second admin account with the same email should fail, not silently overwrite
- `AI_HealthCarePatient` connection string reused from the start (one-database convention, no
  separate `AI_Admin` database ever created) — `Admins` table added via EF migration, additive
  only, verified via `INFORMATION_SCHEMA` diff before/after
- CORS origin set to `http://localhost:4200` (the actual Angular dev port) from the start —
  `HC.AI.Identity.Api`'s CORS has been wrong (`4201`) since it was built and was never corrected;
  didn't fix Identity's here, out of scope, but avoided repeating the mistake
- Tests: 3 `PasswordHasherTests` (hash-is-salted, verify-correct, verify-wrong) — all passing
- **Live smoke test** (real server, real database, real HTTP): signup succeeded, login returned a
  valid JWT, wrong-password login correctly returned 401, duplicate-email signup correctly
  returned 400 — all four verified, then the test row deleted from `Admins`

## Decisions Made
- Kept `HC.AI.Admin.API` as a fully separate solution/deployable from `HC.AI.Identity.Api` (not a
  new controller bolted onto Identity) — matches this project's one-API-per-domain pattern and
  the original US016 scope decision
- No role/permission model beyond "is an admin" — not requested, not invented speculatively

## Local tracking vs. Azure DevOps
Per explicit user instruction ("we will later sync with Azure DevOps"), **ADO was not touched**
this session for US020/TASK018 — Scrum Master is holding this locally. Existing ADO User Story
#48 (from the earlier scaffold-only story, US016) also has not been updated to reflect that the
scaffold is now fully implemented; that's deferred too.

## Handoff
- **Dev QA Agent**: `HC.AI.Admin.API` is live and testable —
  `hc_apis/az/hc_core_apis/HC.AI.Admin.API/`, default dev port `5097` (see `launchSettings.json`),
  endpoints `POST /api/admins/signup` (`fullName`, `email`, `password`) and
  `POST /api/admins/login` (`email`, `password`). No Angular UI is wired to it yet — this is
  backend-only for now.

## References
- [US016](../../product_owner/user_stories/US016_admin_api_setup.md) /
  [US020](../../product_owner/user_stories/US020_admin_signup_login.md)
- [TASK016](../../scrum/tasks/TASK016_US016_admin_api_scaffold.md) /
  [TASK018](../../scrum/tasks/TASK018_US020_admin_signup_login.md)
- [BACKLOG.md](../../product_owner/backlog/BACKLOG.md) — PB033
- [hc_data_source/hc_sql/tables/003_admins_table.sql](../../../hc_data_source/hc_sql/tables/003_admins_table.sql)
