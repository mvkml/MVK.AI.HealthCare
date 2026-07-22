# Epics & Features

Formalizes the Epic → Feature → User Story layer that sat implicit above the flat `PBxxx`
backlog/`USxxx` story lists. Owned by Product Owner; Scrum Master references this when tracking
QA/task work against a story so the hierarchy is always visible, not just the story in isolation.

## Epic: mvkhc Healthcare Platform ([ADO #34](https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_workitems/edit/34))
The single top-level product epic — everything in `BACKLOG.md` currently rolls up under this one
epic. Split into multiple epics later if/when the product grows distinct product lines.

### Feature: Authentication & Identity ([ADO #35](https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_workitems/edit/35))
Login, signup, and the identity backend behind them.

| Item | Type | Title | Status |
|---|---|---|---|
| PB022 | Backlog | Authentication: Login/Signup/Home (US009) — Angular UI, mock-first | Done (UI only) |
| PB023 | Backlog | Real auth backend (`HC.AI.Identity.Api`) | Done — unblocked 2026-07-19 |
| US009 | User Story | Authentication: Login, Sign Up, Home | Done (UI + real backend wired) |
| US017 | User Story | QA Automated Test Coverage for Auth UI (Login & Signup) | Done — Signup and Login suites both closed (14/14 passing) |

### Feature: Persona-Aware Request Routing
Ensuring every prompt request into `HC.AI.MAPI` carries and enforces a real persona, so
model/prompt selection never silently defaults. Distinct from EPIC001's mock config-resolution
work (US011-US013), which is a separate, not-yet-wired mechanism.

| Item | Type | Title | Status |
|---|---|---|---|
| PB034 | Backlog | Persona must be explicit on every prompt request, not a silent default | To Do |
| US021 | User Story | Enforce Persona Requirement Across All Prompt Request Paths | To Do — not yet scoped in detail |
| QA-010 | Task | Playwright coverage for persona -> model routing (`hc_qa/api/hc_ai_mapi`) | Closed — 8/8 passing; case 5 documents the open US021 gap, doesn't close it |

### Feature: QA Governance (cross-cutting)
Test coverage tracking that spans multiple features rather than belonging to one — inventories,
audits, backfilled tracking for pre-existing test suites.

| Item | Type | Title | Status |
|---|---|---|---|
| US022 | User Story | QA Test Coverage Inventory & Untracked Suite Backfill | Done |

## Notes
- Created 2026-07-20 at the user's request, alongside US017, so QA work is always traceable to
  Epic → Feature → Story, not just a flat story reference. See
  `hc_agile/team/scrum_master_agent.md` for the enforcement responsibility.
- Other features (Doctor Chat, Patient Chat, Patient Records, etc.) not yet formalized here —
  add as they come up rather than backfilling everything at once.
