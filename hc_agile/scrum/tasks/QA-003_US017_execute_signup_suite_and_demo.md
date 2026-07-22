# QA-003 - Execute Signup suite + build/run visual signup demo

**US:** US017
**Status:** Closed — Executed, Passed

## Description
Runs `hc_qa/web/aihcweb/tests/signup/signup.spec.ts` (written in QA-002) against the live app
(`aihcweb` on `:4200`) and backend (`HC.AI.Identity.Api` on `:5008`), and builds a visual demo
(`demos/demo3`) that walks the same Signup form in a headed, slowed-down browser.

## Test Steps
1. Start `HC.AI.Identity.Api` (`dotnet run --urls http://localhost:5008`)
2. Start `aihcweb` (`npm start`, proxies `/api/users` → `:5008` per `proxy.conf.json`)
3. `cd hc_qa/web/aihcweb && npx playwright test tests/signup`
4. Build `demos/demo3/signup-demo.ts` and run `npm run demo:signup`

## Result
| Step | Expected | Actual | Result |
|------|----------|--------|--------|
| First run, `npx playwright test tests/signup` | 8/8 pass | 7/8 pass — 1 failure | Fail |
| Failure: "signing up as Patient..." | — | `getByText('My Health Assistant')` matched 2 elements (heading + link) — Playwright strict-mode violation | Locator bug, not a product bug |
| Fix | Narrow to `getByRole('heading', { name: 'My Health Assistant' })` | Applied | Fixed |
| Re-run, `npx playwright test tests/signup` | 8/8 pass | 8/8 pass (6.3s) | Pass |
| `npm run demo:signup` | Headed browser completes signup, stays open | Ran end-to-end, browser left open on success message | Pass |

No product defects found. Nothing logged to `hc_agile/qa/BUG_LOG.md` this cycle — the one failure
was a test-authoring issue (locator specificity), fixed in the same session.

## References
- [QA-002](QA-002_US009_aihcweb_auth_ui_test_cases.md) — test cases this executes
- [US017](../../product_owner/user_stories/US017_qa_auth_ui_test_coverage.md)
