# TASK009 - Wire ChatPage.onSend to the real HC.AI.MAPI Doctor endpoint

**US:** US008
**Status:** Done — 2026-07-18. See
[worklog](../../worklogs/dev_angular/20260718_185023_real_api_incorporation.md).

## Description
`TASK007` built the chat feature module against mock data, explicitly blocked on the US007
endpoint contract. That contract is now delivered — see US008's "Endpoint contract" section for
the full request/response shape, or
`hc_agile/worklogs/dev_semantic_kernel/20260718_180408_module1_doctor_prompt_locked.md` for the
locked module writeup.

## Scope
- Replace `ChatPage`'s mock reply with a real HTTP call to
  `POST http://localhost:5150/api/Doctor/provide-prompt`
- Render `response.content` as the assistant message; surface `response.errorMessage` /
  the 400 validation array on failure
- Loading state must account for real latency (~30s locally, not instant like the mock)
- Base URL should be configurable (environment file), not hardcoded, since it'll differ
  dev/qa/prod

## Deviation from scope — flagged, not silently done
Base URL is **not** in an environment file. `DoctorChatService` calls a relative `/api/...` path,
resolved by an Angular CLI dev-server proxy (`proxy.conf.json`) to `http://localhost:5150` — this
avoids a CORS change on the .NET side, but means dev/qa/prod URL configuration isn't solved, just
deferred. Production topology (same-origin deploy vs. reverse proxy vs. real environment files)
is still an open Architect/DevOps question — see `hc_demo/hc_demo_web/02_aihcweb_architecture_design.md` §2.

## Explicitly out of scope for this task (raise separately if needed)
- Streaming responses — not supported server-side yet
- Any UI for the generation parameters (temperature, maxTokens, etc.) — send request defaults for
  now, only `message` needs to vary with user input
- Database-grounded answers — this endpoint doesn't do patient lookups yet (backlog PB020)
