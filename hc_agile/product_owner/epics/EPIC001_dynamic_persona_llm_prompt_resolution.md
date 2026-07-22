# EPIC001 - Dynamic Persona-Driven LLM + Prompt Resolution

## Goal
Replace the current static, appsettings-bound model/prompt selection in `HC.AI.MAPI` with a
database-driven system where each persona (Doctor, Patient, ...) can have multiple LLM models and
prompts, split into a Classification role (routes the request) and an Executor role (answers it) —
so behavior can change by editing data, not redeploying code.

## Background
Expands PB019 (previously scoped only to "which model for this persona") to also cover dynamic
prompt retrieval (previously just a design note, not tracked) and the classification/executor
model-role split. See the schema proposal:
[`hc_agile/architecture/design_patterns/persona_dynamic_llm_prompt_schema.md`](../../architecture/design_patterns/persona_dynamic_llm_prompt_schema.md).

## Features / User Stories

### Feature: Persona Model & Prompt Configuration Data
- [US011](../user_stories/US011_persona_prompt_type_and_llm_option_config.md) — Persona prompt-type
  and LLM-option configuration tables

### Feature: Dynamic Model & Prompt Resolution at Request Time
- [US012](../user_stories/US012_dynamic_classification_model_routing.md) — Classification-model
  routing to a persona prompt-type
- [US013](../user_stories/US013_dynamic_executor_model_prompt_resolution.md) — Executor model +
  prompt resolution by persona and prompt-type

### Feature: Architecture & Design
- [US019](../user_stories/US019_architect_persona_llm_prompt_schema_design.md) — Architect: schema
  design and open-question sign-off (the design work itself, tracked separately from the tables it
  produces)

## Priority: High
## Status: Draft — design under review, not yet built. See open questions in the schema doc before
any table is created.
