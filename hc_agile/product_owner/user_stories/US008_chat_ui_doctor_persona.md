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
- ~~Blocked on: a stable endpoint contract from Dev .NET Agent for the Doctor persona chat
  endpoint~~ **Delivered 2026-07-18 by Dev Semantic Kernel Agent** — see contract below and
  `hc_agile/worklogs/dev_semantic_kernel/20260718_180408_module1_doctor_prompt_locked.md` (Module
  1, locked, verified live). See `TASK009` for the Angular follow-on work this unblocks.
- ~~Architect to confirm whether this chat feature lives in `hc_ui/aihcweb` alongside US006, or as
  a separate app/module, before Angular work starts.~~ Resolved: proceeding with `hc_ui/aihcweb`.

### Endpoint contract (Module 1, `HC.AI.MAPI`)

- **Base URL:** `http://localhost:5150/api`
- **Recommended endpoint:** `POST /Doctor/provide-prompt` — the validated, structured contract
  (use this one, not the plain `GET /Doctor?message=` string endpoint, which is an earlier,
  unvalidated proof-of-concept kept only for reference)
- **Request body** (only `message` is required — everything else can default/be omitted from the
  UI for now):
  ```json
  { "message": "hi", "maxTokens": 200, "temperature": 0.3, "topP": 0.9, "topK": 40,
    "frequencyPenalty": 0, "presencePenalty": 0, "stopSequences": [], "stream": false, "seed": null }
  ```
- **Success response — `200`:**
  ```json
  { "content": "Hello! How can I assist you today?", "finishReason": "", "promptTokens": 0,
    "completionTokens": 0, "totalTokens": 0, "modelUsed": "qwen2.5:7b", "latencyMs": 30908,
    "isSuccess": true, "errorMessage": "" }
  ```
  Render `content` as the assistant's message. `finishReason`/token counts are currently
  blank/0 (documented gap, not a bug — the Ollama connector doesn't expose them).
- **Validation failure — `400`:** body is a JSON string array, e.g.
  `["Prompt text (Message) is required."]` — surface these as the error/loading-state failure
  case in the chat UI.
- **Latency note:** local Ollama responses currently take ~30s — the loading state should account
  for this, not assume a fast round-trip.
- **Not yet available:** no streaming support (`stream` is accepted in the request but ignored
  server-side); no database-grounded answers yet (see backlog PB020) — this endpoint is a general
  chat completion, not yet a patient-data lookup.

## Acceptance Criteria
- [x] Endpoint contract for the Doctor chat API confirmed — `POST /api/Doctor/provide-prompt`,
      single-response (no streaming yet), see contract above
- [x] Angular app initialized (if not already done by US006) with a chat feature module —
      scaffolded via `@angular/cli@21`; `src/app/features/chat` now holds the built feature module
- [x] Chat UI: message list (user + assistant messages), input box, send action — basic
      ChatGPT-style layout, built as standalone Angular components, matching `design/chat_mockup.html`
      (initial mock data retired now that the real endpoint is wired — see below)
- [x] First end-to-end slice: sending a message through `DoctorChatService` -> `/api/Doctor/provide-prompt`
      returns a real Ollama-generated response — verified live through the Angular dev-server proxy
      (`localhost:4200/api/...` -> `localhost:5150`, the exact path the browser uses), not just a
      direct backend call. Header badge changed to "LLM only — not database-grounded yet" per the
      contract's note that this isn't a patient-data lookup
- [x] Loading/error states handled — composer disables + shows "Sending…" plus a "Thinking… ~30s"
      status line while in flight (per the contract's latency note); network failures, HTTP error
      statuses, and HC.AI.MAPI's 400 validation-array body are all handled distinctly
- [x] Unit tests for the chat component(s) — 9 tests across `ChatPage` (incl. `HttpTestingController`
      coverage of success/network-error/400-validation paths), `Composer`, and root `App`, all passing

## Priority: High
## Status: In Progress — remaining gap is entirely on the backend side (PB020: database-grounded
answers), not the Angular client
## Sprint: Unscheduled — not yet assigned to a sprint plan
## Worklogs:
- [20260717_225719_aihcweb_scaffold_and_chat_mockup.md](../../worklogs/dev_angular/20260717_225719_aihcweb_scaffold_and_chat_mockup.md)
- [20260717_232309_chat_feature_module_mock_data.md](../../worklogs/dev_angular/20260717_232309_chat_feature_module_mock_data.md)
- [20260718_185023_real_api_incorporation.md](../../worklogs/dev_angular/20260718_185023_real_api_incorporation.md)
