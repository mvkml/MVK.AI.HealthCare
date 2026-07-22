# TASK011 - Build Login/Signup/Home UI against mock data

**US:** US009
**Status:** Done — 2026-07-19. See
[worklog](../../worklogs/dev_angular/20260719_190253_mock_auth_login_signup_home.md).

## Description
Angular-only build of Login, Signup, and Home pages, using the `ai_hr` project's
`hr_ui/aihrweb` as a structural reference (re-themed, not copied) and a mock auth service
(no real HTTP call) — same mock-first sequencing as `TASK007` for the chat feature.

## Scope
- `features/auth/{pages/login, pages/signup, data/auth-mock.service.ts, data/auth.guard.ts, models}`
- `features/home/pages/home`
- Route wiring + guard in `app.routes.ts`
- `ChatRail` updated to consume the real mock-logged-in user instead of its own hardcoded persona

## Backlog items this does NOT resolve (tracked in PB022 notes, blocked on the user / Architect)
- DB schema for auth
- Persona/role model decision
- Where the real auth API lives
- Token storage security review (`localStorage` vs httpOnly cookie)
- Real auth backend build
