# QA-001 - aihcweb scaffold smoke test

**US:** US008
**Status:** Executed — Passed

## Description
Validates the Angular scaffold Dev Angular delivered for US008 actually builds and serves, before
any chat feature work is tested. Not a feature test — the chat UI itself doesn't exist yet.

## Test Steps
1. `cd hc_ui/aihcweb && npm install` — expect clean install, 0 vulnerabilities
2. `npx ng build` — expect a successful bundle with no errors
3. `npx ng serve --port 4200` then `GET http://localhost:4200` — expect HTTP 200 with the Angular
   app HTML shell

## Result
| Step | Expected | Actual | Result |
|------|----------|--------|--------|
| npm install | Clean install | 468 packages, 0 vulnerabilities | Pass |
| ng build | Successful bundle | 213.66 kB initial bundle, no errors | Pass |
| ng serve + GET localhost:4200 | HTTP 200 | HTTP 200, app HTML returned | Pass |

No defects found. Nothing logged to `hc_agile/qa/BUG_LOG.md` this cycle.

## Out of scope (tracked in test plan as Planned)
- Chat feature module (message list, input, send) — not built yet
- API integration / "hi" end-to-end slice — blocked on US007 endpoint contract
- Loading/error states, unit tests

See [US008 test plan](../../qa/test_plans/US008_aihcweb_chat_ui_test_plan.md) for the full
coverage backlog this task feeds into.
