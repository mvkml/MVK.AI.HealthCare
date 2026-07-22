# TASK013 - Wire Angular auth UI to the real HC.AI.Identity.Api backend

**US:** US009
**Status:** Done — 2026-07-19. See
[worklog](../../worklogs/dev_angular/20260719_223524_wire_real_identity_api.md). Note: the actual
project on disk is `HC.AI.Identity.Api` (`HC.AI.Identity.*` namespaces) — this task's description
below still says `AI.HR.Api`/`AiHrDbContext` because that's what it was called when written; the
worklog explains the discrepancy.
**Assigned:** Dev Angular Agent

## Why this is unblocked now
PB023 ("Real auth backend") was blocked on a DB schema and an API to call. Both now exist:
`HC.AI.Identity.Api` (`hc_apis/az/hc_core_apis/HC.AI.Identity.Api`) is a working ASP.NET Core API with signup/login/
forgot-password/reset-password/roles endpoints, JWT issuing, and its schema (`Users`, `Roles`,
`OcrDocuments`) has been merged into `AI_HealthCarePatient` — the same database
`AI.HealthCare.Patient.API` already uses. See the Dev .NET / Dev SQL worklogs dated 2026-07-19
for the full merge writeup, and `BACKLOG.md` PB023's notes.

## Description
Replace `AuthMockService` (`hc_ui/aihcweb/src/app/features/auth/data/auth-mock.service.ts`) —
currently pure client-side simulation against a hardcoded demo account list — with real HTTP calls
to `HC.AI.Identity.Api`.

## Scope
- Point login/signup calls at `HC.AI.Identity.Api`'s `POST /api/users/login` and `POST /api/users/signup`
- Decide + implement token storage (`localStorage` vs httpOnly cookie) — still an open decision,
  not made by this task in advance; flag to Architect/Product Owner if not already resolved
- `auth.guard.ts` should check for a real token/session instead of the mock `localStorage` user flag
- ~~Signup's persona/role selection needs to map to `HC.AI.Identity.Api`'s `roleId`~~ — **resolved
  2026-07-19**: roles simplified to `RoleId=1 Doctor`, `RoleId=2 Patient` (migration
  `SimplifyRolesToDoctorPatient`). Signup's default (`UserBL.DefaultSignUpRoleId`) is now `2`
  (Patient). User noted this is a starting point, not final ("later we will change accordingly")
- Forgot/reset-password pages were explicitly out of scope for the mock-only pass (US009) — now
  that the backend supports them, confirm with Product Owner whether to build them in this task or
  a follow-up
- Update/extend existing auth spec files (`login.spec.ts`, `signup.spec.ts`,
  `auth-mock.service.spec.ts` → likely renamed, `auth.guard.spec.ts`) for the real HTTP flow
  (mock the HTTP layer in tests, not the whole service)

## Backlog reference
`BACKLOG.md` PB025.
