# Azure DevOps Wiki — aihcweb Architecture Pages Created

**Status:** Done (first snapshot published).
**Owns:** Dev DevOps Agent (wiki plumbing) / Dev Angular Agent (architecture content).
**Related:** TASK014 / PB027.

## Why this exists
User asked for a place in Azure DevOps to track `aihcweb` (Angular UI) architecture separately
from the rest of the wiki, since architecture reviews will keep happening as the app grows and
each one should be kept as its own record rather than overwritten in place.

## What was done
1. Confirmed a project wiki already exists for **MVK AI Health Care**
   (`MVK-AI-Health-Care.wiki`, type `projectWiki`, auto-provisioned) — found via
   `GET _apis/wiki/wikis`, using the same PAT stored under `AzureDevOps-mvkhc`
   (see [`hc_devops/PAT_SETUP_GUIDE.md`](../../../hc_devops/PAT_SETUP_GUIDE.md)) that TASK014 set
   up for work-item REST calls. Confirmed the same PAT scope also covers wiki read/write — no
   separate token needed.
2. Created a 3-level page hierarchy via `PUT _apis/wiki/wikis/{wikiId}/pages?path=...`:
   - `/aihcweb` — overview page for the Angular UI, links down to Architecture
   - `/aihcweb/Architecture` — index page explaining the versioning approach (numbered snapshot
     pages, not one page edited in place) and listing snapshots
   - `/aihcweb/Architecture/Architecture-1` — first snapshot: current feature/component/routing
     structure of `aihcweb`, two gaps found (inconsistent per-feature routing files; no HTTP
     interceptor attaching the JWT to outgoing API calls), and a proposed target structure
3. Content for `Architecture-1` is the same review already given to the user in-session (ASCII
   structure diagrams), pasted into the wiki verbatim rather than re-derived, so the wiki and the
   conversation record agree.

## Design choice: numbered snapshots, not a single page
Matches the user's stated reason for asking ("architecture changes keep on increasing... helpful
to track the architectures") — `/aihcweb/Architecture` is deliberately an index, and each review
gets a new page (`Architecture-2`, `Architecture-3`, ...) instead of replacing the previous one, so
past state and past findings stay visible.

## Relationship to `hc_agile/` (source of truth)
Same pattern as PB027 generally: `hc_agile/` remains the git-tracked source of truth. This wiki
section is a browsable mirror of one specific architecture review, not a second place where new
architecture decisions get made first. The gaps found (routing inconsistency, missing auth
interceptor) are **documented, not yet fixed** — no code changed as part of this task.

## Verification
- `GET /aihcweb`, `/aihcweb/Architecture`, `/aihcweb/Architecture/Architecture-1` via REST all
  confirmed created (each `PUT` returned the expected `path` in its response).

## Open items
- Gaps documented in Architecture-1 (routing split per feature, HTTP interceptor for the JWT) are
  not yet implemented — separate decision for the user on whether/when to action them.
- No page template/convention doc yet for what "Architecture N" should contain going forward
  (kept informal for this first one).

## Links
- Wiki root: `https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_wiki/wikis/MVK-AI-Health-Care.wiki/?pagePath=/aihcweb`

## References
- [TASK014](../../scrum/tasks/TASK014_PB027_azure_devops_setup.md)
- [BACKLOG.md](../../product_owner/backlog/BACKLOG.md) — PB027
- [PAT_SETUP_GUIDE.md](../../../hc_devops/PAT_SETUP_GUIDE.md)
