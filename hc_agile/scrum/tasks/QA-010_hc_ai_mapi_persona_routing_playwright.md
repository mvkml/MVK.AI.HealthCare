# QA-010 - HC.AI.MAPI Persona -> Model Routing Playwright Coverage

**Epic:** mvkhc Healthcare Platform ([ADO #34](https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care/_workitems/edit/34))
**Feature:** Persona-Aware Request Routing
**US:** [US021](../product_owner/user_stories/US021_persona_required_across_prompt_requests.md) — Enforce Persona Requirement Across All Prompt Request Paths
**Status:** Closed — Executed, Passed (8/8)

## Description
Dev Semantic Kernel Agent wired `PromptRequest.Persona` through `DoctorPromptMapper` ->
`LLMModelBL` -> `LLMOptionsFactory` so `POST /api/Doctor/provide-prompt` resolves a different
Ollama model per persona (`HCDocExecutor` -> `hc-doctor-executor:latest`, `HCPatientExecutor` ->
`hc-patient-executor:1.1`), reading the two new `appsettings.json` sections. Checked the actual
diff (`APIConstants.cs`, `LLMModelBL.cs`, `DoctorPromptMapper.cs`, new `BL/Factory/*`) with the
Semantic Kernel side before writing test cases, per the request to confirm with that agent's work
first rather than assume from the appsettings change alone.

New suite: `hc_qa/api/hc_ai_mapi/` (was a scaffold-only README, now populated — sibling to
`ai_hc_api`/`hc_ai_identity_api`). Executed against the live `HC.AI.MAPI` (`:5150`) + real Ollama
(`:11434`) — no mocking, real ~7B-model generations.

## Test Cases
| # | Case | Result |
|---|---|---|
| 1 | Persona "Doctor" routes to `hc-doctor-executor:latest` | Pass |
| 2 | Persona "Patient" routes to `hc-patient-executor:1.1` | Pass |
| 3 | Persona omitted defaults to `hc-doctor-executor:latest` | Pass |
| 4 | Persona match is case-insensitive ("PATIENT") | Pass |
| 5 | Unrecognized persona ("InsuranceProvider") falls through to `hc-doctor-executor:latest` | Pass |
| 6 | Missing message rejected (400) | Pass |
| 7 | Temperature out of range rejected (400) | Pass |
| 8 | Success response carries latency/token fields | Pass |

## Notes
- **Case 5 is not a passing grade on US021 — it's evidence of the exact gap US021 exists to
  close.** `DoctorPromptMapper` only special-cases an exact match on `"Patient"`; anything else,
  including a typo'd or unrecognized persona, silently falls through to the Doctor executor. This
  test locks in *today's* documented behavior so a future fix (reject unrecognized personas,
  per US021's acceptance criteria) has a test to flip red→green, rather than the change going
  unnoticed. Do not read "8/8 passing" as "US021 is done" — it isn't; only this QA suite's scope is
  closed.
- `PersonaModelResolutionController` / `BL/Persona/*` (also new in this diff) are the unrelated
  EPIC001/US011-US013 mock mechanism, not wired into `provide-prompt` — explicitly out of scope
  here, noted in the new suite's README.
- `PromptResponse.ModelUsed` (traced to `DoctorSemanticProcess.cs:49` ->
  `item.LLMOptions.Model`) is what makes routing assertable from the HTTP response alone, without
  needing to inspect Ollama.

## References
- [US021](../product_owner/user_stories/US021_persona_required_across_prompt_requests.md)
- `hc_qa/api/hc_ai_mapi/README.md`
