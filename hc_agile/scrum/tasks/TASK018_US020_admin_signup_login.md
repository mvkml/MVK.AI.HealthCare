# TASK018 - Implement Admin Sign Up + Login (HC.AI.Admin.API)

**US:** US020
**Status:** Done — 2026-07-20
**Assigned:** Dev .NET Agent

## Description
Scaffold `HC.AI.Admin.API` and implement `POST /api/admins/signup` + `POST /api/admins/login`,
backed by a new `Admins` table in the shared `AI_HealthCarePatient` database.

## What Was Done
See [worklog](../../worklogs/dev_dotnet/20260720_193000_admin_api_signup_login_implemented.md) for
full detail — solution structure, EF migration, live smoke test results.

## Backlog reference
`BACKLOG.md` PB033.

## Handoff
- **Dev QA Agent** notified — API is live and testable at `hc_apis/az/hc_core_apis/HC.AI.Admin.API/`,
  endpoints `POST /api/admins/signup` and `POST /api/admins/login`
- **Scrum Master** holding US020/TASK018 locally — ADO sync explicitly deferred by user instruction
