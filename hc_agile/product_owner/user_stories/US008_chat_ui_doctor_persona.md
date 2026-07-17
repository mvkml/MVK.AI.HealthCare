# US008 - Chat UI for Doctor Persona (Angular)

**As a** Doctor
**I want to** ask questions through a ChatGPT-style chat window in the browser
**So that** I can use the natural-language assistant (US007) without calling the API directly via Swagger/Postman

## Background
Depends on the Chat REST API being built in `HC.AI.MAPI` under
[US007](US007_healthcare_ai_assistant.md). This story covers only the Angular client that calls
that API once its first end-to-end slice (`UCDoctorController` greeting response) is available —
it does not duplicate or replace US007's backend acceptance criteria. Frontend home is
`hc_ui/aihcweb`, currently an empty shell (no `package.json`, no components) per
[US006](US006_health_web_portal.md)/PB006. This chat UI is a distinct feature area from US006's
patient-records portal pages, though both may end up in the same Angular app.

## Dependency / Handoff
- Blocked on: a stable endpoint contract from Dev .NET Agent for the Doctor persona chat endpoint
  (request/response shape, base URL — see `HC.AI.MAPI` entry in
  [REFERENCE_LINKS.md](../../REFERENCE_LINKS.md)).
- ~~Architect to confirm whether this chat feature lives in `hc_ui/aihcweb` alongside US006, or as
  a separate app/module, before Angular work starts.~~ Resolved: proceeding with `hc_ui/aihcweb`.

## Acceptance Criteria
- [ ] Endpoint contract for the Doctor chat API confirmed with Dev .NET Agent (request/response
      schema, streaming vs single-response, base URL)
- [x] Angular app initialized (if not already done by US006) with a chat feature module —
      scaffolded via `@angular/cli@21`; `src/app/features/chat` now holds the built feature module
- [x] Chat UI: message list (user + assistant messages), input box, send action — basic
      ChatGPT-style layout, built as standalone Angular components against **mock data**
      (`src/app/features/chat/data/chat-mock-data.ts`), matching `design/chat_mockup.html`
- [ ] First end-to-end slice: sending "hi" through the UI returns the same greeting response
      proven by US007's `UCDoctorController` hello-world path — currently mocked (canned reply,
      no HTTP call); blocked on the endpoint contract above
- [ ] Loading/error states handled (API unavailable, slow response) — nothing to load/error on
      yet since there's no real HTTP call
- [x] Unit tests for the chat component(s) — 6 tests across `ChatPage`, `Composer`, and root `App`,
      all passing (`ng test`)

## Priority: High
## Status: In Progress
## Sprint: Unscheduled — not yet assigned to a sprint plan
## Worklogs:
- [20260717_225719_aihcweb_scaffold_and_chat_mockup.md](../../worklogs/dev_angular/20260717_225719_aihcweb_scaffold_and_chat_mockup.md)
- [20260717_232309_chat_feature_module_mock_data.md](../../worklogs/dev_angular/20260717_232309_chat_feature_module_mock_data.md)
