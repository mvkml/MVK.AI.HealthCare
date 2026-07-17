# aihcweb — Angular scaffold + chat UI design mockup (2026-07-17)

Covers US008's first acceptance criterion ("Angular app initialized with a chat feature module")
and the design-reference step ahead of it.

## What was built

**Design mockup** (static HTML, no Angular yet): `hc_ui/aihcweb/design/chat_mockup.html`
— ChatGPT-style layout for the Doctor persona: conversation-history rail, message stream with
patient-lookup/aggregate-query examples matching US007's acceptance criteria, `Read-only` /
`Grounded in patient records` badges, and a `Source:` tag on each assistant reply. Published as an
artifact for visual review before any Angular component work started.

**Angular scaffold**: `hc_ui/aihcweb` was an empty shell (only `.gitignore`, plus pre-existing
empty `src/app/features` and `src/app/shared` directories from earlier project setup — no
`package.json`, no components). Generated fresh via `@angular/cli@21` (routing enabled, CSS
styles, standalone app, no SSR) into a scratch directory, then merged into the existing folder so
the pre-existing empty `features`/`shared` dirs were preserved rather than overwritten.

- `@angular/cli@latest` (v22.x) refused to run — requires Node `^22.22.3 || ^24.15.0 || >=26.0.0`,
  and the local Node is `v24.12.0`. Pinned to `@angular/cli@21.2.19`, which accepts
  `>=24.0.0` and works with the installed Node version.
- `npm install` — 468 packages, 0 vulnerabilities.
- `npx ng build` — verified the scaffold builds clean (213.66 kB initial bundle) before calling
  this done.

## Open items (per US008)

- Endpoint contract for the Doctor chat API not yet confirmed with Dev .NET / Dev Semantic Kernel
  — US007 backend is still "In Progress". The chat feature module/component build is blocked on
  this.
- Chat feature module itself (components, routing into `src/app/features`) not yet built — this
  session only covers app initialization + the design reference.

## References

- [US008](../../product_owner/user_stories/US008_chat_ui_doctor_persona.md)
- [US006](../../product_owner/user_stories/US006_health_web_portal.md) — shares the same
  `hc_ui/aihcweb` app
- Source: [`hc_ui/aihcweb/`](../../../hc_ui/aihcweb/)
