# Naming Convention

**Status:** Draft — raised by Dev Angular Agent (2026-07-17), pending Architect review/ratification.
Owned by the Architect Agent per `hc_agile/team/architect_agent.md`; this file didn't exist yet
when the first real Angular feature module (`hc_ui/aihcweb/src/app/features/chat`, US008) needed
one, so it defaults to the plain Angular style guide rather than inventing a project-specific
scheme. Amend or replace freely — nothing below is locked in.

## Angular (`hc_ui/aihcweb`)
- Files: kebab-case (`chat-page.ts`, `message-item.html`)
- Components: standalone (no `NgModule`), class names in PascalCase without a `Component` suffix
  (`ChatPage`, not `ChatPageComponent`) — matches what `@angular/cli@21`'s own generator produces
  for the root `App` component
- Selector prefix: `app-` (the CLI default, set in `angular.json`)
- Feature folder shape: `features/<feature>/{pages,components,data,models}` — `pages` for
  routed/container components, `components` for presentational ones, `data` for mock/static data,
  `models` for TypeScript interfaces

## Not yet covered
- .NET (`HC.AI.MAPI`, `AI.HC.Api`, etc.) — no convention written down yet, though existing code
  already follows standard C#/.NET conventions (PascalCase types, `I`-prefixed interfaces)
- SQL (`hc_data_source/hc_sql`)
- Cross-project naming (e.g. how backlog/story/task IDs map to folder or branch names)
