# TASK010 - Demo 1: REST API walkthrough (Doctor persona, Module 1)

**US:** US007
**Status:** Ready

## Description
User is giving a live demo of the REST API only (no Angular UI) — Module 1's
`POST /api/Doctor/provide-prompt` end-to-end flow. Not a new build task, just tracking the demo
milestone and its scope so expectations are recorded.

## Scope — what IS being demoed
- Full request pipeline: validation -> model selection -> LLM execution (Ollama) -> structured
  response, live against `http://localhost:5150/api/Doctor/provide-prompt`
- See [Module 1 writeup](../../worklogs/dev_semantic_kernel/20260718_180408_module1_doctor_prompt_locked.md)
  for the verified request/response shape

## Scope — what is NOT being demoed (explicitly out of scope, per PB020)
- Real patient-data lookups / database-grounded answers — `HealthcareQueryTool` is not wired into
  this flow yet
- Guardrail enforcement (allow-listed tables/read-only/record limits) — not wired in yet
- The Angular UI (`aihcweb`) — this is a REST-API-only demo; UI integration is being prepared in
  parallel (see Dev Angular worklog) for a later demo, not this one

## Follow-up
Dev Angular's real-endpoint wiring is tracked as
[TASK009](TASK009_US008_wire_real_doctor_endpoint.md) (now Done) — not a blocker for this task.
