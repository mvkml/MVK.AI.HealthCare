# QA-009 - Admin Signup Playwright test cases

**US:** US018 (the QA coverage story) — tests `/admin/signup`, part of the same Admin Identity feature as [US014](../../product_owner/user_stories/US014_admin_login.md)/[US015](../../product_owner/user_stories/US015_admin_signup.md)
**Status:** Closed — Executed, Passed (5/5)

## Description
Playwright test cases for `/admin/signup`, written during the `tests/` folder restructuring
(module-wise by user type — see `hc_agile/wiki/qa/hc_qa_folder_structure/v2_2026-07-21.md`),
originally scoped out of QA-005 but added once the restructuring created a natural home for it
(`tests/admin-user/admin-signup/`).

## Test Cases
| # | Case | Result |
|---|---|---|
| 1 | Valid signup shows a success message | Pass |
| 2 | Mismatched passwords show a client-side error | Pass |
| 3 | Password under 8 characters shows a client-side error | Pass |
| 4 | Missing required fields shows a client-side error | Pass |
| 5 | Signing up with an existing email is rejected (mock does not upsert, unlike the real Identity API) | Pass |

## References
- [US018](../../product_owner/user_stories/US018_qa_admin_login_test_coverage.md)
- [QA-005](QA-005_US014_admin_login_test_cases.md) — sibling task, Admin Login
