# Real API incorporation — aihcweb wired to HC.AI.MAPI (2026-07-18)

Triggered by the user prepping a "Demo 1: REST API only" walkthrough of Module 1
(`POST /api/Doctor/provide-prompt`, see
[dev_semantic_kernel worklog](../dev_semantic_kernel/20260718_180408_module1_doctor_prompt_locked.md)),
and asking Dev Angular to have the UI's real API incorporation ready in parallel for a future
UI-attached demo. Cross-checked scope with Architect (PB020: don't oversell this as the
database-grounded patient-query feature) and Scrum Master (tracked as
[TASK010](../../scrum/tasks/TASK010_US007_demo1_rest_api.md) for the demo itself; this worklog is
the actual implementation of [TASK009](../../scrum/tasks/TASK009_US008_wire_real_doctor_endpoint.md),
which was independently raised with the same scope while this was already in progress).

## What was built

- `src/app/features/chat/data/doctor-chat.service.ts` — `DoctorChatService.providePrompt()`,
  typed `PromptRequest`/`PromptResponse` mirroring `HC.AI.MAPI.Models.Prompt` exactly (confirmed
  against the endpoint contract that landed in US008 while this was in progress — field names,
  defaults, and the 400-validation-array response shape all matched what was already built)
- `provideHttpClient()` added to `app.config.ts`
- `proxy.conf.json` + `angular.json` `serve.options.proxyConfig` — proxies `/api/*` from the
  Angular dev server to `http://localhost:5150`, so the browser never makes a cross-origin request
  and no CORS change was needed on the .NET side (stays inside Dev Angular's ownership boundary,
  doesn't require a Dev .NET/Semantic Kernel change)
- `ChatPage.onSend` — replaced the mocked canned reply with a real `DoctorChatService.providePrompt()`
  call: `isSending`/`errorMessage` signals, three distinct error paths (network failure via
  `status === 0`, HC.AI.MAPI's 400 validation string-array body, other HTTP errors)
- `Composer` — new `disabled` input, textarea/button disabled + "Sending…" label while a request
  is in flight
- Header badge changed from the old mockup's "Read-only / Grounded in patient records" to "LLM
  only — not database-grounded yet" — the old badges were no longer accurate once this hits the
  real endpoint, and PB020 is explicit that this must not be presented as the patient-query feature
- Added a "Thinking… ~30s" status line while sending, since the contract documents ~30s local
  Ollama latency and a bare disabled button would otherwise look broken

## Verification

- `ng build` and `ng test` (9 tests, up from 6 — added HTTP success/network-error/400-validation
  coverage via `HttpTestingController`) both clean
- **Live, through the actual browser-equivalent path**: `HC.AI.MAPI` and Ollama were already
  running locally, so rather than only testing against a mocked HTTP layer, curled
  `POST http://localhost:4200/api/Doctor/provide-prompt` (i.e. through the Angular dev server's
  proxy, not directly at :5150) and got a real 200 with a genuine Ollama-generated response —
  proves the exact request path the browser will take, not just that the backend works in
  isolation
- Still not done: no actual browser/screenshot verification (no browser automation tool available
  this session) — recommend opening `http://localhost:4200` and sending a real message before
  treating this as fully UI-verified

## Explicitly out of scope (per Architect/PB020)
Real patient-data grounding is a backend gap, not an Angular one — `HealthcareQueryTool` isn't
wired into this flow yet. The badge/copy changes above exist specifically so the UI doesn't imply
otherwise.

## References
- [US008](../../product_owner/user_stories/US008_chat_ui_doctor_persona.md) — endpoint contract section
- [TASK009](../../scrum/tasks/TASK009_US008_wire_real_doctor_endpoint.md) — the task this worklog completes (base-URL-via-environment-file deviation noted there)
- [TASK010](../../scrum/tasks/TASK010_US007_demo1_rest_api.md) — Demo 1 scope
- [BACKLOG.md](../../product_owner/backlog/BACKLOG.md) — PB020 caveat
