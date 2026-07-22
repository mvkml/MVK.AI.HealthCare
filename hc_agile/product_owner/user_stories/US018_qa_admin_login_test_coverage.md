# US018 - QA Automated Test Coverage for Admin Login

**Epic:** Admin Management ([ADO #44](https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_workitems/edit/44)) · **Feature:** Admin Identity ([ADO #45](https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_workitems/edit/45))
— sibling to [US014](US014_admin_login.md) (ADO #46, the feature-delivery story this tests)

**As a** QA Engineer
**I want to** have automated Playwright test cases for the Admin login page
**So that** the separate Admin entry point (distinct from Doctor/Patient login) keeps working as
expected, including that it stays isolated from the Doctor/Patient session

## Background
US014 delivered `/admin/login` (mock-only — `AdminAuthMockService`, no backend Admin role exists
yet, see BACKLOG PB031). This story adds QA coverage for it under
`hc_qa/web/aihcweb/tests/admin-login/`, one module folder per page, same convention as
[US017](US017_qa_auth_ui_test_coverage.md)'s Login/Signup coverage.

## Acceptance Criteria
- [x] Playwright test cases written for Admin Login (`tests/admin-login/admin-login.spec.ts`) —
      valid/invalid credentials, empty-field validation, route-guard redirect
- [x] Test case confirming Admin and Doctor/Patient sessions are isolated (an Admin session must
      not unlock `/home`) — this is the actual point of US014's "separate entry point" requirement
- [x] Suite executed against the live app — 11/11 passing (6 admin-login + 5 admin-signup)

## Tasks

| Task | Description | Status |
|---|---|---|
| [QA-005](../../scrum/tasks/QA-005_US014_admin_login_test_cases.md) | Write and execute Admin Login Playwright test cases | Closed — Passed 6/6 |
| [QA-009](../../scrum/tasks/QA-009_US014_admin_signup_test_cases.md) | Write and execute Admin Signup Playwright test cases | Closed — Passed 5/5 |

## Out of scope
- Real backend testing — `AdminAuthMockService` is mock-only, nothing to test beyond the UI/guard
  behavior already covered

## Priority: Medium
## Status: Done — all acceptance criteria met, all tasks closed locally
## Sprint: Unscheduled — not yet assigned to a sprint plan

## Azure DevOps
Not yet created there — drafting locally first. Let me know if you want this pushed to ADO as a
new User Story under Feature #45, alongside #46/#47.
