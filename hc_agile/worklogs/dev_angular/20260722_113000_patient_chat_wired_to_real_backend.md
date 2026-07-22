# Dev Angular Agent — Work Log
## Date: 2026-07-22
## Subject: Wire PatientChatPage to the real HC.AI.MAPI backend (US023/TASK020)

## What Happened
`HC.AI.MAPI` gained a real Patient chat pipeline (`PatientController` + supporting BL/Services/AL
classes — see the Dev Semantic Kernel Agent worklog). Wired the Angular side to actually use it,
mirroring exactly how Doctor's chat was moved off its mock (TASK009/US008 pattern):

- Added `patient-chat.service.ts` (`PatientChatService`) — same `PromptRequest`/`PromptResponse`
  shape as `doctor-chat.service.ts`, POSTs to `/api/Patient/provide-prompt`,
  `persona: 'Patient'` hardcoded client-side (same reasoning as Doctor's: `PatientChatPage` is
  only reachable by a logged-in Patient via route guard, so it's correct for every real caller
  today, not read dynamically from the auth session)
- `patient-chat-page.ts` — swapped the injected service, added `errorMessage` signal + the same
  three-way `subscribe` error handling as `ChatPage` (network unreachable / 400 validation array /
  other HTTP error)
- `patient-chat-page.html` — badge updated from "Mock demo — no Patient backend yet" to "LLM
  only — not database-grounded yet" (matches Doctor's real-backend copy), added the error banner
  block
- Deleted `patient-chat-mock.service.ts` + its spec — confirmed via grep nothing else referenced
  the mock before removing
- Rewrote `patient-chat-page.spec.ts` against `HttpTestingController` (the old version asserted on
  the mock's fixed 600ms-delayed canned reply, which no longer applies) — same test shape as
  `chat-page.spec.ts`: real API call + reply, network-failure error, 400-validation error, persona
  rail, logout

## Verification
- `npx ng test --watch=false`: 15 test files / 57 tests passing, no regressions elsewhere
- Confirmed the existing `proxy.conf.json`'s `/api -> localhost:5150` rule already covers
  `/api/Patient/...` — no proxy changes needed
- Live end-to-end through the actual dev-server proxy (`localhost:4200`, not a direct backend
  call): real reply, `modelUsed: "hc-patient-executor:1.1"`, `isSuccess: true` — confirms the whole
  chain (Angular -> proxy -> HC.AI.MAPI -> Ollama) works, not just the backend in isolation

## References
- [US023](../../product_owner/user_stories/US023_wire_real_patient_chat_backend.md)
- [TASK020](../../scrum/tasks/TASK020_US023_wire_real_patient_chat_backend.md)
- [US010](../../product_owner/user_stories/US010_patient_chat_ui.md) — original mock-first UI story, updated to point here
