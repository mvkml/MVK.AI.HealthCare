# US013 - Executor Model + Prompt Resolution by Persona and Prompt-Type

**As a** persona's classified request
**I want to** be handed to the Executor model + prompt matching its resolved prompt-type
**So that** the actual response comes from the right model/agent for that specific kind of request,
not a one-size-fits-all prompt per persona

## Background
Part of [EPIC001](../epics/EPIC001_dynamic_persona_llm_prompt_resolution.md). Depends on
[US011](US011_persona_prompt_type_and_llm_option_config.md) (config/seed data) and
[US012](US012_dynamic_classification_model_routing.md) (this story consumes the
`PersonaPromptType.Code` that US012 produces). This is where "multiple LLM models and prompts per
persona" actually gets used — one Executor model/prompt per prompt-type, not per persona.

## Acceptance Criteria
- [x] Given a persona + prompt-type code, the system resolves that persona+type's active Executor
      model + prompt — **from the mock config provider**, not the DB
- [ ] Executor model runs with the resolved prompt and returns the response through the existing
      response contract — NOT built; not wired into the live Doctor `provide-prompt` endpoint
      (deliberate — see TASK017's "Deliberately NOT done" section)
- [x] If no Executor model/prompt is configured for a given persona+type combination, behavior is
      defined — the mock reports `IsResolved = false` (verified live: unknown code -> 404) rather
      than guessing error-vs-default; **the actual product decision (error vs. persona-level
      default) still needs Product Owner sign-off**, this is just "don't crash, don't guess"

## Priority: High
## Status: Partially done (mock) — config resolution built (TASK017); not wired into the live
Doctor flow
## Sprint: Unscheduled
## Worklogs:
- [20260720_180000_persona_resolution_mock.md](../../worklogs/dev_dotnet/20260720_180000_persona_resolution_mock.md)
