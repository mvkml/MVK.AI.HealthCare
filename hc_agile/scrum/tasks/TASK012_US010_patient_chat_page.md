# TASK012 - Build Patient chat page (mock-first) + promote shared chat components

**US:** US010
**Status:** Done — 2026-07-19. See
[worklog](../../worklogs/dev_angular/20260719_193855_patient_chat_page.md).

## Description
Build a separate `PatientChatPage`, reusing (not duplicating) the message-rendering components
already built for Doctor chat.

## Scope
- Promote `ChatRail`, `MessageList`, `MessageItem`, `Composer`, and the `ChatMessage` model from
  `features/chat/` to `shared/` — genuine cross-feature reuse now exists, not hypothetical
- `ChatRail`'s `historyItems`/`persona` inputs changed from defaulted (hardcoded Doctor mock data)
  to `input.required<...>()` — a shared component shouldn't default to one feature's mock content
- `features/patient-chat/` — mock data, `PatientChatMockService`, `PatientChatPage`
- `/patient-chat` route (guarded)
- `Home` page's Patient action card now links to the real page
