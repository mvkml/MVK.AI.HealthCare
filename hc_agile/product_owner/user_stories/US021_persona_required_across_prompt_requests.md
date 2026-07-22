# US021 - Enforce Persona Requirement Across All Prompt Request Paths

**As a** system architect
**I want to** every prompt request into `HC.AI.MAPI` to carry an explicit, validated persona value
**So that** model/prompt selection always branches on the real caller's persona instead of silently
falling through to a default

## Background
Tracks [PB034](../backlog/BACKLOG.md), raised as its own high-priority item — distinct from (but
overlapping) PB019/PB032/EPIC001. The resolution *mechanism* already exists and is live-wired into
the real `Doctor/provide-prompt` endpoint (not the EPIC001 mock path US011-US013 cover):
- `LLMOptionsFactory.GetLLMOptions(string persona)`
  (`HC.AI.MAPI.BL/Factory/LLMOptionsFactory.cs`) binds a config section keyed by persona name
- `DoctorPromptMapper.ToPromptItem` (`HC.AI.MAPI.Services/Mapping/DoctorPromptMapper.cs`) picks
  `ModelKey` via `request.Persona == "Patient" ? PatientExecutorPersonaName :
  DoctorExecutorPersonaName` — **anything that isn't literally `"Patient"` (including an empty
  string, or the new Admin persona) silently falls through to the Doctor executor**
- `PromptRequest.Persona` / `PromptItem.Persona` fields exist; Angular's
  `doctor-chat.service.ts` now sends `persona: 'Doctor'` hardcoded

This is exactly the gap PB034 flags: nothing requires the field to be set, and nothing rejects an
unrecognized value — it defaults instead of failing loud. The newly-added Admin persona
(`HC.AI.Admin.API`, PB033/US020) has no prompt/LLM involvement at all yet, so is an easy path to
silently mis-route if this endpoint were ever reused for it.

## Acceptance Criteria
- [ ] `PromptRequest.Persona` is required (empty/missing → 400, not a silent default)
- [ ] `DoctorPromptMapper.ToPromptItem`'s persona → `ModelKey` mapping rejects unrecognized persona
      values instead of falling through to `DoctorExecutorPersonaName`
- [ ] Every current request path into `HC.AI.MAPI` (today: Doctor chat only) is verified to set
      `Persona` explicitly — not just the happy path
- [ ] Decide and document what should happen if the Admin persona ever calls a prompt endpoint
      (reject outright, since Admin has no LLM involvement per PB033/US020 — vs. silently routing
      to a Doctor/Patient executor)
- [x] Angular `doctor-chat.service.ts`'s hardcoded `persona: 'Doctor'` reviewed — **confirmed
      intentional**: `ChatPage` is only reachable by a logged-in Doctor (route-guarded), so the
      value is correct for every real caller today. Flagging the nuance rather than calling it
      fully resolved: it's a literal hardcoded string, not read from
      `AuthService.currentUser()?.persona` — harmless while only Doctor reaches this page, but
      would need to become dynamic if this component/service is ever reused for another persona.

## Priority: High
## Status: Partially done — the *mechanism* (persona -> ModelKey -> different Ollama model) is
live-wired and QA-verified (QA-010, 8/8 passing incl. the exact "falls through silently" gap this
story exists to close). The *enforcement* ACs above (require + reject unrecognized + Admin
decision) are still open — do not read QA-010's pass rate as this story being done, see that
suite's own notes.
## Sprint: Unscheduled
## Worklogs:
- [20260721_180000_persona_model_selection_wired.md](../../worklogs/dev_semantic_kernel/20260721_180000_persona_model_selection_wired.md) — mechanism wiring (TASK019)
- QA-010 (`hc_agile/scrum/tasks/QA-010_hc_ai_mapi_persona_routing_playwright.md`) — test coverage
