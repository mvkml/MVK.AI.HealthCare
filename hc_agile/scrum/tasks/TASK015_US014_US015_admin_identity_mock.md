# TASK015 - Build Admin Identity pages (Login + Sign-Up), mock-first

**Status:** Done — 2026-07-20. See
[worklog](../../worklogs/dev_angular/20260720_170000_admin_identity_mock.md).
**US:** US014, US015

**Numbering note:** originally drafted as US011/US012, renumbered to US014/US015 after
discovering concurrent work had already claimed US011 (`US011_persona_prompt_type_and_llm_option_config.md`,
`US011_qa_auth_ui_test_coverage.md`) and US012 (`US012_dynamic_classification_model_routing.md`)
for the unrelated EPIC001 (PB032) work. Filename kept as `TASK015` since that number wasn't taken.
**Status:** In Progress
**Assigned:** Dev Angular Agent

## Why this exists
User wants an Admin persona, ultimately to manage Doctor and Patient details — but that
management capability isn't scoped yet. First step is the entry point: a separate `/admin/...`
area with its own login/signup, structurally identical to the existing Doctor/Patient auth pages.

## Scope
- New feature folder `features/admin-auth/`, mirroring `features/auth/` (models, data, pages)
- `AdminAuthMockService` — pure client-side mock (same shape the original `AuthMockService` had
  before `HC.AI.Identity.Api` existed): `currentAdmin` signal, `isLoggedIn` computed, `login()`,
  `signUp()`, `logout()`, `localStorage`-backed session under its own key so it doesn't collide
  with the real Doctor/Patient session
- `AdminLogin` page (`/admin/login`) — mirrors `Login`, no persona selector needed
- `AdminSignup` page (`/admin/signup`) — mirrors `Signup`, no persona selector needed (always Admin)
- `adminAuthGuard` — same shape as `authGuard`, checks `AdminAuthMockService.isLoggedIn()`
- Minimal `/admin/home` landing stub, guarded, so login has somewhere to go — explicitly a
  placeholder ("Admin environment — Doctor/Patient management coming soon"), not the real
  management console
- Unit tests for the new service/pages, same coverage shape as the existing auth spec files

## Explicitly out of scope
- Doctor/Patient record management UI — undefined scope, separate future story
- Real backend — no `Admin` role exists in `HC.AI.Identity.Api` today; wiring this to a real API
  is blocked on that role + admin-management endpoints being designed

## Backlog reference
`BACKLOG.md` PB031.
