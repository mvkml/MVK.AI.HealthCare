# US022 - QA Test Coverage Inventory & Untracked Suite Backfill

**Epic:** mvkhc Healthcare Platform ([ADO #34](https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_workitems/edit/34)) · **Feature:** QA Governance (cross-cutting — not tied to one product feature)

**As a** QA Engineer
**I want to** inventory every testing-related work item across every dev agent, and formally
track anything that exists but isn't tracked yet
**So that** test coverage is visible and traceable from one place, instead of scattered across
worklogs and code with no scrum record

## Background
Requested directly: visit every agent, gather anything test-related, and file it under QA's own
name in the scrum tracking. Surveyed: all xUnit test projects (`hc_apis/`, `hc_ai_in/`), all
Angular `*.spec.ts` files (`hc_ui/aihcweb/src/`), all Playwright suites (`hc_qa/`), and every
agent's worklog folder for test-related mentions.

## Findings summary
- **Angular unit tests** (16 files, 58 `it(` blocks) — already documented via Dev Angular's own
  worklogs (counts match exactly); not a QA-owned gap, since QA validates but doesn't author these
  per `hc_agile/team/dev_qa_agent.md`. No new task created for these.
- **Backend xUnit tests** (4 projects, `hc_apis/`) — never validated or tracked by QA before.
  Backfilled as [QA-006](../../scrum/tasks/QA-006_backend_unit_test_coverage_audit.md).
- **`ai_hc_api` Playwright suite** (17 tests, pre-existing, QA-authored) — never had a task file.
  Backfilled as [QA-007](../../scrum/tasks/QA-007_ai_hc_api_playwright_tracking.md).
- **`hc_ai_identity_api` Playwright suite** (19 tests, executed earlier this session) — never had
  a task file. Backfilled as [QA-008](../../scrum/tasks/QA-008_identity_api_playwright_tracking.md).
- Assorted manual/live smoke checks in `dev_dotnet`, `dev_sql`, `dev_semantic_kernel` worklogs —
  informational only, not codified as reusable tests; not backfilled as tasks (nothing to track
  beyond the worklog entry itself).

## Tasks

| Task | Description | Status |
|---|---|---|
| [QA-006](../../scrum/tasks/QA-006_backend_unit_test_coverage_audit.md) | Audit all 4 xUnit test projects for coverage | Executed — 1 gap flagged (`HC.AI.Admin.Api.Tests` has no real coverage yet) |
| [QA-007](../../scrum/tasks/QA-007_ai_hc_api_playwright_tracking.md) | Backfill tracking for `ai_hc_api` suite (17 tests) | Written — not yet executed this session |
| [QA-008](../../scrum/tasks/QA-008_identity_api_playwright_tracking.md) | Backfill tracking for `hc_ai_identity_api` suite (19 tests) | Closed — Passed (19/19) |

## Priority: Low (housekeeping/traceability, not new product risk)
## Status: Done — inventory complete, all identified gaps backfilled with tracking
## Sprint: Unscheduled — not yet assigned to a sprint plan

## Azure DevOps
Not yet created there — drafting locally first, per usual convention.
