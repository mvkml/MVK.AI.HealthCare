# US017 - QA Automated Test Coverage for Auth UI (Login & Signup)

**Epic:** mvkhc Healthcare Platform ([ADO #34](https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_workitems/edit/34)) · **Feature:** Authentication & Identity ([ADO #35](https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_workitems/edit/35)) — see
[EPICS_AND_FEATURES.md](../backlog/EPICS_AND_FEATURES.md)

**Azure DevOps:** [User Story #36](https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_workitems/edit/36) (Closed)

**As a** QA Engineer
**I want to** have automated Playwright test cases and a visual demo for the Login and Signup
pages
**So that** regressions in the real (`HC.AI.Identity.Api`-backed) auth flow are caught
automatically, and the flow can be demoed quickly without repeating manual steps

## Background
US009 delivered the Login/Signup/Home UI (mock-first), then TASK013 wired it to the real
`HC.AI.Identity.Api` backend. This story covers the QA layer on top of that delivered feature —
test cases and demo scripts under `hc_qa/web/aihcweb/`, organized one module folder per page
(`tests/login/`, `tests/signup/`), per the QA folder convention in
`hc_agile/team/dev_qa_agent.md`. It does not re-scope or re-test US009's UI itself, only adds
automated coverage and a repeatable demo path for it.

## Acceptance Criteria
- [x] Playwright test cases written for Login (`tests/login/login.spec.ts`) — valid/invalid
      credentials, empty-field validation, route-guard redirects
- [x] Playwright test cases written for Signup (`tests/signup/signup.spec.ts`) — valid signup,
      validation errors, persona assignment, backend upsert behavior
- [x] Signup suite executed against the live app + backend — 8/8 passing
- [x] Login suite executed against the live app + backend — 6/6 passing
- [x] Visual demo scripts (`demos/demo1`, `demo2`, `demo3`) — headed, slowed-down browser runs for
      login→Doctor Chat, login→Patient Chat, and Signup, so the flow can be shown without manual
      typing

## Tasks

| Task | Description | Status |
|---|---|---|
| [QA-002](../../scrum/tasks/QA-002_US009_aihcweb_auth_ui_test_cases.md) | Write Login + Signup Playwright test cases | Written — [ADO Task #37](https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_workitems/edit/37) (Closed) |
| [QA-003](../../scrum/tasks/QA-003_US017_execute_signup_suite_and_demo.md) | Execute Signup suite; build + run `demo3` (visual signup demo) | **Closed** — Passed (8/8, after 1 locator fix) — [ADO Task #38](https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_workitems/edit/38) |
| [QA-004](../../scrum/tasks/QA-004_US017_execute_login_suite.md) | Execute Login suite | **Closed** — Passed (6/6) — [ADO Task #39](https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_workitems/edit/39) |

## Priority: Medium
## Status: Done — all acceptance criteria met, all tasks closed locally
## Sprint: Unscheduled — not yet assigned to a sprint plan
## Worklog: [20260720_scrum_log_qa_auth_ui_coverage.md](../../worklogs/scrum_master/)
