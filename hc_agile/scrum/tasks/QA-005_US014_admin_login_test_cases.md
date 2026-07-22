# QA-005 - Admin Login Playwright test cases

**US:** US014 (tests this feature) — also tracked under [US018](../../product_owner/user_stories/US018_qa_admin_login_test_coverage.md) (the QA coverage story)
**Status:** Closed — Executed, Passed (6/6, after fixing a test-fixture bug — see Notes)

## Description
Playwright test cases for `/admin/login`, built in US014 on `AdminAuthMockService` (mock-only,
localStorage-backed — no `HC.AI.Identity.Api` Admin role exists yet, see BACKLOG PB031). Organized
under `hc_qa/web/aihcweb/tests/admin-login/`, per the QA folder convention in
`hc_agile/team/dev_qa_agent.md`.

**Note (2026-07-21):** relocated to `tests/admin-user/admin-login/` — see
`hc_agile/wiki/qa/hc_qa_folder_structure/v2_2026-07-21.md`. Path above is as originally written.

## Test Cases

| # | Case | Status |
|---|---|---|
| 1 | Valid credentials redirect to `/admin/home` | Pass |
| 2 | Wrong password shows an error, stays on `/admin/login` | Pass |
| 3 | Unknown email shows an error | Pass |
| 4 | Empty email/password shows client-side validation | Pass |
| 5 | Unauthenticated visitor to `/admin/home` redirected to `/admin/login` (route guard) | Pass |
| 6 | An admin session does not grant access to `/home` (Doctor/Patient area) — confirms session isolation | Pass |

## Setup dependencies
- `hc_ui/aihcweb` running on `http://localhost:4200` (`npm start`)
- No backend dependency — `AdminAuthMockService` is localStorage-only
- `fixtures/admin-auth.ts` — `seedAdminAccount()` writes directly to the `hc_admin_accounts`
  localStorage key (mirrors the mock service's own storage shape), since there's no API to seed
  against

## Notes
- Case 6 exists because it's the actual point of US014 ("a separate entry point ... distinct from
  Doctor/Patient login") — without it, nothing would catch a regression where the two sessions
  accidentally shared state.
- `AdminAuthMockService.signUp` rejects duplicate emails (unlike the real `HC.AI.Identity.Api`,
  which upserts) — confirmed by `tests/admin-user/admin-signup/admin-signup.spec.ts` (5/5 passing,
  written in the folder-restructuring pass).
- **Bug found and fixed (2026-07-21):** `fixtures/admin-auth.ts`'s `seedAdminAccount()` called
  `crypto.randomUUID()` inside a `page.evaluate()` callback — that bare `crypto` reference resolved
  to the file's Node-side `crypto` import (captured by closure during bundling), not the browser's
  global `crypto`, throwing `ReferenceError: _crypto is not defined` in-page. Fixed by generating
  the id in Node before the callback and passing it in as data, rather than referencing anything
  ambiguous from inside browser-evaluated code. Broke all 4 tests that called `seedAdminAccount()`
  (3 in this file, 1 in `admin-signup.spec.ts`) — a test-infrastructure bug, not a product defect,
  not logged to `BUG_LOG.md`.

## References
- [US018](../../product_owner/user_stories/US018_qa_admin_login_test_coverage.md)
