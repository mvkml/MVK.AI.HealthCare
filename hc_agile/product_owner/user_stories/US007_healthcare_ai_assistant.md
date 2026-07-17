# US007 - Healthcare AI Assistant (Doctor Persona, Natural-Language Query)

**As a** Doctor
**I want to** ask natural-language questions through a chat interface (patient lookup, recent
patients, clinical data)
**So that** I get accurate, database-grounded answers without needing a separate screen or
endpoint per question type

## Background
Builds on the design locked in
[`healthcare_ai_assistant_mcp_ollama_design.md`](../../architecture/design_patterns/healthcare_ai_assistant_mcp_ollama_design.md)
and the layered architecture scaffolded in `hc_ai_in/mapi/HC.AI.MAPI` (Service Layer -> Agent
Layer -> Prompt/Guardrail/LLM/Tool -> Business Layer -> Repository -> EF -> SQL Server). Doctor is
the primary client persona (see `2026-07-17_service_layer_architecture_discussion.md`) and gets a
dedicated, higher-accuracy prompt/guardrail treatment.

## Acceptance Criteria
- [ ] First feature (basic, end-to-end): `UCDoctorController` responds to a simple greeting
      ("hi") with a basic, correct response — proves the full request path (Controller -> Agent
      Layer -> ... -> Business Layer) works for the Doctor persona, the same way
      `HelloWorldController` -> `HC.AI.MAPI.BL` already proved the plain Controller -> BL path
- [ ] Extend to real single-patient lookup ("patient details for John Smith")
- [ ] Extend to aggregate/list queries ("recent visiting patients", "how many patients")
- [ ] Every answer is grounded only in real query results returned by the Tool Layer — no
      fabricated patient data
- [ ] Guardrail Layer restricts what a Doctor persona's query is allowed to touch (allow-listed
      tables/columns, read-only, record limits) before it reaches the Business Layer

## Priority: High
## Status: In Progress
## Sprint: Unscheduled — not yet assigned to a sprint plan (only `sprint_01` exists so far)
