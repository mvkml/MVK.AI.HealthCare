# TASK014 - Set up Azure DevOps process (client/demo showcase board)

**Backlog:** PB027
**Status:** In Progress — PAT/API access confirmed working; one generic Epic #1 + Issue #2 created
(not part of the mirroring structure below, needs reconciliation); PB001-PB026 mirroring not
started. Wiki section for `aihcweb` architecture started separately, see
[worklog](../../worklogs/dev_devops/20260720_143000_wiki_aihcweb_architecture.md).
**Assigned:** Dev DevOps Agent

## Why this exists
User wants a client/leadership-facing showcase of team progress — a live Azure Boards Kanban
board with sprint burndown, not just the `hc_agile/` markdown tables. Cost was the initial
concern; resolved to Azure DevOps Basic plan (free for ≤5 users, no billing risk since paid
features like Test Plans require explicit purchase). `hc_agile/` remains the source of truth;
Azure Boards mirrors it for presentation purposes only.

## Description
User has created the Azure DevOps project **MVK AI Health Care**. Dev DevOps Agent to work
directly with the user on:
- Organization URL and access (PAT with Work Items Read & Write scope, if API/CLI-driven setup
  is wanted, vs. user creating items manually in the UI)
- Board/process template choice (Basic process recommended — matches the free plan and the
  Epic > Feature > User Story > Task hierarchy already used conceptually in `hc_agile/`)
- Mirroring `hc_agile/product_owner/backlog/BACKLOG.md` (PB001–PB026) into Boards:
  - Epics/Features grouping for related PB items (e.g. all `HC.AI.MAPI` Doctor Chat PBxxx under
    one Feature)
  - User Stories from `hc_agile/product_owner/user_stories/*.md` (US007-US010 etc.)
  - Test Case work items from `hc_agile/qa/test_plans/` and `BUG_LOG.md`
- Sprint/iteration setup so a burndown chart is actually meaningful, not just a Kanban column dump

## Scope boundary
This task is about the Azure DevOps board/process setup itself, not re-doing any of the
already-completed engineering work (PB013-PB026) — it's a presentation-layer mirror of what
`hc_agile/` already tracks.

## Backlog reference
`BACKLOG.md` PB027.
