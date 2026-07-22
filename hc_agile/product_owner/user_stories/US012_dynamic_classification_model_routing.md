# US012 - Classification-Model Routing to a Persona Prompt-Type

**As a** persona's incoming request (Doctor, Patient, ...)
**I want to** be routed through that persona's classification model first
**So that** the system knows which prompt-type/agent should actually answer it, instead of a single
undifferentiated prompt handling every request

## Background
Part of [EPIC001](../epics/EPIC001_dynamic_persona_llm_prompt_resolution.md). Depends on
[US011](US011_persona_prompt_type_and_llm_option_config.md) (config tables + seed data must exist
first). This story covers only the classification step — resolving and running the Classification
model/prompt for a persona and getting back a `PersonaPromptType.Code`. It does not cover running
the Executor model (see [US013](US013_dynamic_executor_model_prompt_resolution.md)).

## Acceptance Criteria
- [x] Given a persona (`RoleId`), the system resolves that persona's active Classification model +
      prompt — **from the mock config provider**, not the DB (DB still blocked on US011's open
      questions) and not appsettings
- [ ] Classification model runs and its output maps to a valid `PersonaPromptType.Code` for that
      persona — NOT built; this mock only proves the config-resolution step, not an actual Ollama
      classification call + output parsing
- [ ] Unmapped/unrecognized classification output has a defined fallback behavior (needs Product
      Owner sign-off on what that fallback is — not assumed here; the mock returns
      `IsResolved = false` for unresolvable lookups rather than guessing)

## Priority: High
## Status: Partially done (mock) — config resolution built (TASK017); running the Classification
model + parsing its output is not
## Sprint: Unscheduled
## Worklogs:
- [20260720_180000_persona_resolution_mock.md](../../worklogs/dev_dotnet/20260720_180000_persona_resolution_mock.md)
