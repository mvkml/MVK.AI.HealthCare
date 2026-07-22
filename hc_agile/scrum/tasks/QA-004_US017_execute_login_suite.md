# QA-004 - Execute Login suite

**US:** US017
**Status:** Closed — Executed, Passed

## Description
Run `hc_qa/web/aihcweb/tests/login/login.spec.ts` (written in QA-002, 6 cases) against the live
app + `HC.AI.Identity.Api` backend, same setup as [QA-003](QA-003_US017_execute_signup_suite_and_demo.md).

## Test Steps
1. Confirm `HC.AI.Identity.Api` (`:5008`) and `aihcweb` (`:4200`) are running
2. `cd hc_qa/web/aihcweb && npx playwright test tests/login`

## Result
| Step | Expected | Actual | Result |
|------|----------|--------|--------|
| `npx playwright test tests/login` | 6/6 pass | 6/6 pass (4.5s) | Pass |

No product defects found. Nothing logged to `hc_agile/qa/BUG_LOG.md` this cycle.

## References
- [QA-002](QA-002_US009_aihcweb_auth_ui_test_cases.md) — test cases this will execute
- [US017](../../product_owner/user_stories/US017_qa_auth_ui_test_coverage.md)
