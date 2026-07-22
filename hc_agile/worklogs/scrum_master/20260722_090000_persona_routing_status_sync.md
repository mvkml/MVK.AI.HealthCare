# Scrum Master — Work Log
## Date: 2026-07-22
## Subject: Status sync on persona-routing work (TASK019/PB034/US021) — QA already notified, story updated

## What Happened
User asked (as Scrum Master) to make sure QA had been told about the persona-routing changes and
that a user story existed for the completed Angular-side piece, marked accordingly, before
resuming execution.

Checked current state before creating anything new, since a lot of concurrent work had landed:
- **QA already notified and already acted.** [QA-010](../../scrum/tasks/QA-010_hc_ai_mapi_persona_routing_playwright.md)
  exists, references [US021](../../product_owner/user_stories/US021_persona_required_across_prompt_requests.md),
  and reports an 8/8-passing Playwright suite (`hc_qa/api/hc_ai_mapi/`) run live against
  `HC.AI.MAPI` + real Ollama — including a test that specifically locks in the exact "unrecognized
  persona silently falls through to Doctor" gap US021 exists to close. No new QA task needed; QA
  is already ahead of this sync.
- **US021 existed but understated what was actually done.** It tracked the full PB034 scope
  (require persona, reject unrecognized values, decide the Admin case) as one un-checked list, with
  no worklog links. Updated it to:
  - Check off the one AC that's genuinely complete and verified (Angular's
    `doctor-chat.service.ts` sending `persona: 'Doctor'` explicitly — confirmed intentional, with
    the caveat that it's a hardcoded literal, not read from the logged-in user's actual persona,
    which is fine while only Doctor reaches that page but not if the component is ever reused)
  - Changed Status from "To Do" to "Partially done", explicitly warning not to read QA-010's
    passing suite as the whole story being closed (matching QA-010's own notes) — the enforcement
    ACs (require + reject + Admin decision) are still open and unowned
  - Linked both the Semantic Kernel worklog (mechanism) and QA-010 (coverage) as Worklogs

## Action
- Updated [US021](../../product_owner/user_stories/US021_persona_required_across_prompt_requests.md)
  per above — did not mark the whole story Done, since that would misstate what's actually
  finished (QA-010 explicitly warns against exactly that reading).
- No new QA task filed — QA-010 already covers the notification/coverage ask.

## Not yet done
- US021's three remaining ACs (require persona server-side, reject unrecognized values, decide the
  Admin-persona case) — not assigned to an agent yet.
- Separately, Dev Angular Agent has a real Patient-chat backend (`PatientController` + supporting
  BL/Services/AL classes, mirroring Doctor's Module 1) in progress in the same session this sync
  happened in — not yet built/verified/wired to the Angular UI. Out of scope for this sync;
  continuing after this status check.

## References
- [US021](../../product_owner/user_stories/US021_persona_required_across_prompt_requests.md)
- [QA-010](../../scrum/tasks/QA-010_hc_ai_mapi_persona_routing_playwright.md)
- [TASK019](../../scrum/tasks/TASK019_PB034_persona_model_selection.md)
