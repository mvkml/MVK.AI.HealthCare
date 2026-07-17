# QA — aihcweb scaffold smoke test (2026-07-17)

First QA pass in this project (previously "Observing" only — see capability tracker). Handed off
from Dev Angular after the `aihcweb` scaffold + design mockup for US008 landed.

## What was tested
Scaffold-level smoke test only, not a feature test: `npm install`, `ng build`, `ng serve` +
`GET localhost:4200`. All three passed — see [QA-001](../../scrum/tasks/QA-001_US008_aihcweb_scaffold_smoke_test.md).
No defects found, nothing logged to `BUG_LOG.md`.

## QA backlog set up this session
- `hc_agile/qa/BUG_LOG.md` — master defect register, created (empty)
- `hc_agile/qa/test_plans/US008_aihcweb_chat_ui_test_plan.md` — living coverage table against
  US008's acceptance criteria, most rows still "Planned" since the chat feature itself isn't built
- `hc_agile/qa/test_results/` — created, empty, for future per-cycle result snapshots

## Next QA cycle triggers
- Dev Angular builds the chat feature module -> write test cases for message list / input / send
- US007 endpoint contract confirmed -> write the "hi" end-to-end UI test case
- Loading/error state handling built -> write those test cases

## References
- [US008](../../product_owner/user_stories/US008_chat_ui_doctor_persona.md)
- [Dev Angular worklog this depends on](../dev_angular/20260717_225719_aihcweb_scaffold_and_chat_mockup.md)
