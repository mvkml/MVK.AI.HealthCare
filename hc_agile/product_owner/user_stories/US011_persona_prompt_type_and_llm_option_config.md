# US011 - Persona Prompt-Type and LLM-Option Configuration Tables

**As a** system administrator
**I want to** define, per persona, which prompt-types exist and which LLM models are available for
Classification vs. Executor roles
**So that** model/prompt behavior can be configured through data instead of code changes

## Background
Part of [EPIC001](../epics/EPIC001_dynamic_persona_llm_prompt_resolution.md). Depends on the schema
proposal being finalized —
[`persona_dynamic_llm_prompt_schema.md`](../../architecture/design_patterns/persona_dynamic_llm_prompt_schema.md)
reuses the existing `Roles` table as the persona-group concept rather than introducing a duplicate;
this needs explicit sign-off before implementation, along with the other open questions listed in
that doc (single- vs. multi-role-per-user, actual prompt-type values, fallback semantics).

## Acceptance Criteria
- [ ] Open design questions resolved (single-role-per-user vs. many-to-many, actual
      `PersonaPromptType` values per persona, `Priority`/fallback semantics)
- [ ] `PersonaPromptType`, `PersonaLLMOption`, `PersonaPrompt` tables created (Dev SQL Agent),
      FK'd to existing `Roles`
- [x] Seed data for at least the Doctor persona's Classification model/prompt and one Executor
      model/prompt, so US012/US013 have something real to resolve against — **done as mock data**
      (`PersonaLlmConfigMockProvider`, in-memory), not real DB rows; still blocked on the open
      design questions above before this can become real DDL + seed data

## Priority: High
## Status: Mock implemented (TASK017) — real DB table creation still blocked on design sign-off
## Sprint: Unscheduled
## Worklogs:
- [20260720_180000_persona_resolution_mock.md](../../worklogs/dev_dotnet/20260720_180000_persona_resolution_mock.md)
