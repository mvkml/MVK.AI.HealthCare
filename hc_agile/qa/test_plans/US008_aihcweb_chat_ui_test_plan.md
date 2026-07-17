# Test Plan — US008 Chat UI for Doctor Persona (`aihcweb`)

**Linked story:** [US008](../../product_owner/user_stories/US008_chat_ui_doctor_persona.md)
**Linked backlog item:** PB012

This plan tracks QA coverage against US008's acceptance criteria as they're built. Rows move from
Planned -> Written -> Executed -> Passed/Failed as the underlying feature lands — this file is
the QA backlog for the chat UI, updated every cycle rather than replaced.

| # | Acceptance criterion | Test case | Coverage Status |
|---|---|---|---|
| 1 | Endpoint contract confirmed | N/A — contract review, not a test case | Blocked (contract not yet confirmed) |
| 2 | Angular app initialized with chat feature module | [QA-001](../../scrum/tasks/QA-001_US008_aihcweb_scaffold_smoke_test.md) — scaffold builds & serves | Executed — Passed (scaffold only; chat feature module itself not yet built) |
| 3 | Chat UI: message list, input box, send action | Not yet written — module doesn't exist yet | Planned |
| 4 | First end-to-end "hi" slice through the UI | Not yet written — blocked on #1 | Planned |
| 5 | Loading/error states (API unavailable, slow response) | Not yet written — blocked on #3/#4 | Planned |
| 6 | Unit tests for chat component(s) | QA validates coverage exists; doesn't write the unit tests itself (Dev Angular's responsibility) | Planned |

## Notes
- First cycle (2026-07-17) only covers what actually exists today: the raw Angular scaffold and
  the static `design/chat_mockup.html` reference. No chat feature, no API integration yet — most
  rows above stay Planned until Dev Angular builds them.
- Re-run and update this table every QA cycle rather than creating a new file per cycle.
