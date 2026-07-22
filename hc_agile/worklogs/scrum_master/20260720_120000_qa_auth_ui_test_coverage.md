# Scrum Master — Work Log
## Date: 2026-07-20
## Subject: QA automated test coverage for Auth UI (Login & Signup) — US017 created

## What Happened
QA Agent wrote and executed Playwright test cases for the real (`HC.AI.Identity.Api`-backed)
Login and Signup pages under `hc_qa/web/aihcweb/tests/`, organized one folder per module, plus
visual demo scripts under `hc_qa/web/aihcweb/demos/`.

## Action
- Raised **US017** — "QA Automated Test Coverage for Auth UI (Login & Signup)", a quality/test
  story separate from US009 (which delivered the feature itself)
- Raised task table under US017:
  - **QA-002** — write Login + Signup test cases — Written
  - **QA-003** — execute Signup suite + build/run `demo3` — Executed, Passed (8/8, after fixing
    one locator-specificity bug found during the run — not a product defect)
  - **QA-004** — execute Login suite — To Do, not yet run this session
- No defects logged to `hc_agile/qa/BUG_LOG.md` this cycle

## ADO note (still pending)
User separately asked for an Azure DevOps user story for this work. Flagged in QA-002: no PAT is
configured for `dev.azure.com/mvishnukiran05/MVK AI Health Care` yet, and it queues behind the
already-pending "Azure DevOps Environment Setup" story. US017 (this local story) is the record of
intent until the PAT exists.

## References
- [US017](../../product_owner/user_stories/US017_qa_auth_ui_test_coverage.md)
- [QA-002](../../scrum/tasks/QA-002_US009_aihcweb_auth_ui_test_cases.md)
- [QA-003](../../scrum/tasks/QA-003_US017_execute_signup_suite_and_demo.md)
- [QA-004](../../scrum/tasks/QA-004_US017_execute_login_suite.md)
