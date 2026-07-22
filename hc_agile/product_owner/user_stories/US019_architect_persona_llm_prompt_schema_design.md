# US019 - Architect: Design Schema for Persona-Driven Dynamic LLM + Prompt Resolution

**As a** Architect Agent
**I want to** propose the table schema for persona-based Classification/Executor model and prompt
resolution, with open design questions explicitly flagged rather than assumed
**So that** Dev SQL Agent has a reviewed, sign-off-ready design to implement against instead of
building on unverified assumptions

## Background
Part of [EPIC001](../epics/EPIC001_dynamic_persona_llm_prompt_resolution.md) (PB032). This story
covers the design/authoring work itself — distinct from
[US011](US011_persona_prompt_type_and_llm_option_config.md) (the config tables being built) and
[US012](US012_dynamic_classification_model_routing.md)/[US013](US013_dynamic_executor_model_prompt_resolution.md)
(the resolution behavior) — this is the Architect's own deliverable, tracked so it shows up in
Azure DevOps like any other unit of work once synced.

## Acceptance Criteria
- [x] Schema proposal written, reusing the existing `Roles` table instead of introducing a
      duplicate persona-group concept — see
      [`persona_dynamic_llm_prompt_schema.md`](../../architecture/design_patterns/persona_dynamic_llm_prompt_schema.md)
- [x] All proposed tables carry consistent audit columns (`CreatedDate`, `UpdatedDate`, `IsActive`)
- [x] Open design questions (single- vs. multi-role-per-user, real prompt-type values, fallback
      semantics, `Roles` naming) explicitly listed for user sign-off, not silently assumed
- [ ] User sign-off received on the open questions above
- [ ] Synced to Azure DevOps (Epic #44's structure or equivalent) once sign-off is in

## Priority: High
## Status: In Progress — design delivered, pending user sign-off and ADO sync
## Sprint: Unscheduled
## Worklogs: none yet
