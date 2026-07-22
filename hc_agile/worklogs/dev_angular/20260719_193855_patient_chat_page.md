# Patient chat page — separate page, shared components (2026-07-19)

Implements [TASK012](../../scrum/tasks/TASK012_US010_patient_chat_page.md) / US010. Follows the
architecture discussion from earlier the same session: presented three options (persona-branch in
one `ChatPage`, separate page + shared presentational layer, or fully separate feature module) as
text flow charts, recommended the middle option, user confirmed with "create a different chat page
for the patient."

## What was built

**Promoted to `shared/`** (previously lived only under `features/chat/`):
- `shared/components/{chat-rail, composer, message-item, message-list}`
- `shared/models/chat-message.model.ts`
- `ChatRail`'s `historyItems`/`persona` inputs changed from `input(MOCK_...)` (defaulted to
  Doctor's mock data) to `input.required<...>()` — now exports its own `ChatHistoryItem`/
  `ChatRailPersona` interfaces instead of importing Doctor-specific mock data. Every caller
  (`ChatPage`, new `PatientChatPage`) now passes its own data explicitly.
- The `.app`/`.chat`/`.chat-head`/`.badge`/etc. page-shell CSS (previously duplicated-in-waiting)
  moved into global `styles.css` alongside the existing `.auth-shell`/`.auth-card` shared block —
  same rationale, now actually shared by two pages instead of one.

**New — `features/patient-chat/`:**
- `data/patient-chat-mock-data.ts` — mock history scoped to *the patient's own record only*
  (next appointment, current medications) — deliberately no aggregate/cross-patient query, since
  that's the actual guardrail distinction from Doctor chat, not just cosmetic content
- `data/patient-chat-mock.service.ts` — `PatientChatMockService`, keyword-matched canned replies,
  explicitly mock (no HTTP call at all — there is no Patient backend, unlike Doctor's live Module 1)
- `pages/patient-chat-page/` — same container shape as `ChatPage` (messages/isSending signals,
  logout), header badge reads "Mock demo — no Patient backend yet" instead of Doctor's
  "LLM only — not database-grounded yet", since the two pages are honest about different levels of
  backend reality
- `/patient-chat` route added, behind the same `authGuard` as `/home` and `/chat`
- `Home`'s Patient action card now links to `/patient-chat` instead of the earlier "not built yet"
  placeholder from US009

## Verification
- `ng build` — clean; `patient-chat-page` is its own lazy chunk (~3.5 kB), `chat-page`'s chunk
  shrank now that the shared components moved out of it
- `ng test` — 36/36 passing across 11 spec files (up from 29) — added specs for the promoted
  `ChatRail` (now with required-input setup), `PatientChatMockService`, `PatientChatPage`, and
  updated `Home`'s patient-persona test to match the new content
- Live: dev server got stuck on a stale template-not-found error mid-edit (a normal HMR race —
  the `.ts` file existed a moment before its `.html`/`.css` siblings) and didn't self-recover;
  restarted it cleanly and confirmed `/`, `/login`, `/signup`, `/home`, `/chat`, `/patient-chat`
  all serve 200
- **Not done**: no actual browser/screenshot verification (no browser automation tool available
  this session)

## References
- [US010](../../product_owner/user_stories/US010_patient_chat_ui.md)
- [TASK012](../../scrum/tasks/TASK012_US010_patient_chat_page.md)
- [US008](../../product_owner/user_stories/US008_chat_ui_doctor_persona.md) / [US009](../../product_owner/user_stories/US009_authentication_login_signup_home.md) — prior work this builds on
