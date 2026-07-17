# Chat feature module — built with mock data (2026-07-17)

Covers the remaining Angular half of US008's "message list / input box / send action" acceptance
criterion, following on from the earlier scaffold + design mockup session
([worklog](20260717_225719_aihcweb_scaffold_and_chat_mockup.md)).

## What was built

`src/app/features/chat/` — standalone Angular 21 components (no NgModule, matching the CLI's own
generated `App` component style: signals, standalone, new `@if`/`@for` control flow):

- `models/chat-message.model.ts` — `ChatMessage` shape (text, result lists, detail fields, source
  tag) covering both example exchanges from the design mockup
- `data/chat-mock-data.ts` — mock chat history, history-rail items, persona — **no HTTP call, no
  backend dependency**. Explicitly commented as mock, pending the US007 endpoint contract
- `components/message-item`, `components/message-list`, `components/composer`,
  `components/chat-rail` — presentational components
- `pages/chat-page` — container component: holds `signal<ChatMessage[]>` state seeded from mock
  data, appends a user message + a canned mock assistant reply on send
- `chat.routes.ts` wired into `app.routes.ts` (root redirects to `/chat`, lazy-loaded)
- `app.html`/`app.ts` stripped down to just `<router-outlet />` — removed the generated Angular
  welcome-page boilerplate

## A layout bug worth flagging for other Angular work in this repo

Copying the mockup's CSS almost verbatim initially broke message alignment: `align-self` on the
inner `<article class="msg">` had no effect, because the actual flex container's children are the
`<app-message-item>` **host elements**, not the inner markup. Fixed by binding
`[class.msg-user]`/`[class.msg-assistant]` onto the host itself (via the `host: {...}` metadata)
and targeting `:host(.msg-user)` in CSS. Every component host also needed an explicit `:host`
`display` rule — custom elements default to `display: inline`, which silently breaks flex/grid
layouts that assume `block`/`flex` children.

## Verification

- `npx ng build` — clean, `chat-page`/`chat.routes` now lazy chunks (13.16 kB / 140 bytes)
- `npx ng test --watch=false` — 3 spec files, 6 tests, all passing (`ChatPage` mock-data rendering
  + send behavior, `Composer` emit + empty-guard behavior, root `App` smoke test)
- `npx ng serve` — verified HTTP 200 on a live port, then stopped
- **Not done**: no visual/screenshot verification — no browser automation tool available in this
  session. Recommend the user open `http://localhost:4200` after `npm start` to eyeball it
  against the design mockup before treating this as fully verified.

## Open gap flagged for Architect

No `hc_agile/architecture/decisions/NAMING_CONVENTION.md` exists yet (referenced as "source of
truth" in `architect_agent.md` but never written). This session defaulted to plain Angular style
guide conventions (kebab-case files, `pages`/`components`/`data`/`models` feature-folder split,
standalone components) — flagging so Architect can either ratify this as the convention or correct
it before more Angular feature folders get built the same way.

## Still open per US008
- Endpoint contract from Dev .NET / Dev Semantic Kernel — still blocking the real "hi" end-to-end
  slice (currently mocked)
- Loading/error states — not built (nothing to load/error on yet, since there's no real HTTP call)

## References
- [US008](../../product_owner/user_stories/US008_chat_ui_doctor_persona.md)
- [Design mockup](../../../hc_ui/aihcweb/design/chat_mockup.html)
- [QA-001 scaffold smoke test](../../scrum/tasks/QA-001_US008_aihcweb_scaffold_smoke_test.md) — QA's next cycle should now cover this feature module
