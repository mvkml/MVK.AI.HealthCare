# QA-002 - aihcweb Login/Signup UI test cases

**US:** US009 (tests this feature) — also tracked under [US017](../../product_owner/user_stories/US017_qa_auth_ui_test_coverage.md) (the QA coverage story)
**Status:** Written — execution tracked separately in QA-003 (Signup) / QA-004 (Login)

## Description
Playwright UI test cases for the Login and Signup pages built in US009, now that they call the
real `HC.AI.Identity.Api` backend (TASK013) instead of `AuthMockService`. Organized as one folder
per module under `hc_qa/web/aihcweb/tests/`, per the QA folder convention in
`hc_agile/team/dev_qa_agent.md`.

**Note (2026-07-21):** relocated to `tests/site-user/doctor-user/` and `tests/site-user/patient-user/`
(each with its own `login/`/`signup/` subfolder, content duplicated per persona) — see
`hc_agile/wiki/qa/hc_qa_folder_structure/v2_2026-07-21.md`. Paths below are as originally written.

## Test Cases

### `tests/login/login.spec.ts`
| # | Case | Status |
|---|---|---|
| 1 | Valid credentials redirect to `/home` | Written |
| 2 | Wrong password shows an error, stays on `/login` | Written |
| 3 | Unknown email shows an error | Written |
| 4 | Empty email/password shows client-side validation | Written |
| 5 | Unauthenticated visitor to `/home` redirected to `/login` (route guard) | Written |
| 6 | Unauthenticated visitor to `/chat` redirected to `/login` (route guard) | Written |

### `tests/signup/signup.spec.ts`
| # | Case | Status |
|---|---|---|
| 1 | Valid signup shows a success message | Written |
| 2 | Mismatched passwords show a client-side error | Written |
| 3 | Password under 8 characters shows a client-side error | Written |
| 4 | Missing required fields shows a client-side error | Written |
| 5 | Signing up with an existing email is accepted (backend upserts, does not reject) | Written |
| 6 | Whitespace-only full name is treated as missing (client-side error) | Written |
| 7 | Invalid email format is rejected by the backend, not client-side (no client email-format check exists) | Written |
| 8 | Signing up as Patient lands that account on the Patient home view after login | Written |

## Setup dependencies
- `hc_ui/aihcweb` running on `http://localhost:4200` (`npm start`)
- `HC.AI.Identity.Api` running on `http://localhost:5008`
  (`hc_apis/az/hc_core_apis/HC.AI.Identity.Api/HC.AI.Identity.Api && dotnet run --urls http://localhost:5008`)
- `npm install` once from `hc_qa/web/aihcweb`

## Notes
- `fixtures/auth.ts` seeds test users directly against the API (`seedUser`), bypassing the UI, so
  login tests don't depend on the signup flow also passing.
- Case 5 under Signup documents real (not assumed) backend behavior found while writing these
  tests: `SignUp` is an `Upsert` keyed on `Email`, so there's no "account already exists" error
  path today — worth a Product Owner/Architect call on whether that's intended long-term.
- `hc_qa/web/aihcweb/demos/demo1` and `demo2` predate this task and still reference the old mock's
  hardcoded demo accounts (`doctor@demo.health` / `patient@demo.health`) — not verified against
  the real DB, flagged in `hc_qa/web/aihcweb/README.md`.

## Scrum Master note
User asked for an Azure DevOps user story for this QA work. Flagged, not created: no PAT is
configured yet for `dev.azure.com/mvishnukiran05/MVK AI Health Care` (Work Items Read & Write
scope required), and there's already one ADO item pending the same PAT (the "Azure DevOps
Environment Setup" Epic/Feature/Story). This QA-002 task is the local record of intent — user will
check on this and it queues behind the same PAT dependency.

## Out of scope (not written this pass)
- Forgot/reset-password UI (no pages built yet — backend supports it, per TASK013)
- Home page and persona-aware chat-link assertions beyond what login case 1 already checks
- Signup with an already-in-use RoleId edge cases
