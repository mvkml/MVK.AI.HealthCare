# US020 - Admin Sign Up + Login Endpoints (HC.AI.Admin.API)

**As an** Admin
**I want to** sign up and log in through a dedicated backend API
**So that** an Admin account can be created and authenticated independently of the Doctor/Patient
identity system

## Background
Follows [US016](US016_admin_api_setup.md) (scaffold). User provided the concrete endpoint scope
this time: sign up and login only. Built as a genuinely separate persona/table from
`HC.AI.Identity.Api`'s `Users` — no `Company`/`RoleId` fields carried over (those were HR-domain
leftovers on the Identity side); `Admins` is a clean, purpose-built table.

## Scope
- `HC.AI.Admin.API` solution scaffolded at `hc_apis/az/hc_core_apis/HC.AI.Admin.API/` — layered
  the same way as `HC.AI.Identity.Api` (Api / BL / EF / Models / Repositories, plus a Tests
  project), `HC.AI.Admin.*` namespaces throughout, net8.0 (aligned with the rest of the codebase's
  APIs, template defaulted to net9.0)
- `POST /api/admins/signup` — full name, email, password (min 8 chars); rejects duplicate emails
- `POST /api/admins/login` — email + password; verifies against PBKDF2-HMAC-SHA256 hash, issues a
  JWT (`Issuer: HC.AI.Admin.Api`, `Audience: HC.AI.Admin.Web`, distinct signing secret from
  Identity's)
- `Admins` table added to the shared `AI_HealthCarePatient` database (one-database convention,
  same as `HC.AI.Identity.Api`'s `Users`/`Roles` tables) via EF Core migration — additive only
- CORS fixed to the correct Angular dev port (`4200`) from the start — `HC.AI.Identity.Api` had
  this wrong (`4201`) and it was never corrected
- Unit tests (`PasswordHasherTests`) + live smoke test: signup, login (valid + wrong password),
  duplicate-email rejection — all verified working against the real database

## Explicitly out of scope for this story
- Forgot/reset password — not requested
- Wiring the existing mock Admin Angular UI (US014/US015) to this API — separate future story,
  Dev QA Agent notified this API now exists so test planning can start
- `Admin` role/permission model beyond "is an admin" — no granular permissions requested

## Priority: High
## Status: Done — 2026-07-20
## Sprint: Unscheduled
## ADO sync: **Deferred per user instruction** ("we will later sync with Azure DevOps") — tracked
locally only for now; Scrum Master holds this until told to mirror it
## Worklog: [20260720_193000_admin_api_signup_login_implemented.md](../../worklogs/dev_dotnet/20260720_193000_admin_api_signup_login_implemented.md)
