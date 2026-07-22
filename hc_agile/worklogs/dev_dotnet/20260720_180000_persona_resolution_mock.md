# EPIC001 Resolution Mechanism — Implemented with Mock Config Models (2026-07-20)

Implements [TASK017](../../scrum/tasks/TASK017_US011_US012_US013_persona_resolution_mock.md) /
US011 / US012 / US013 / BACKLOG PB032.

## Why mock, not real DB
[persona_dynamic_llm_prompt_schema.md](../../architecture/design_patterns/persona_dynamic_llm_prompt_schema.md)
(the proposed `PersonaPromptType`/`PersonaLLMOption`/`PersonaPrompt` tables) has four explicit open
questions before it can become real DDL: single- vs. multi-role-per-user, actual prompt-type
values per persona, fallback semantics for multiple active options, and a `Roles` naming question.
None of those are answered yet. Rather than block all of US011-US013 on that sign-off, implemented
the resolution *mechanism* against mock in-memory data shaped like the proposed tables — same
mock-first pattern as the Angular UI work throughout this project (build the plumbing, swap in the
real data source once it exists, keep the public shape identical so the swap is mechanical).

## What was built (`HC.AI.MAPI`)
- `HC.AI.MAPI.Models.Persona/` — `PersonaPromptType`, `PersonaLlmOption`, `PersonaPromptRecord`,
  `ModelRole` constants, `PersonaModelResolutionResult`
- `HC.AI.MAPI.BL.Persona.PersonaLlmConfigMockProvider` (`IPersonaLlmConfigProvider`) — in-memory
  data: Doctor (`RoleId 1`) only, one placeholder prompt-type (`"General"`). The schema doc
  explicitly says real prompt-type values aren't guessed at — this mock doesn't invent them
  either; `"General"` exists only to prove the Classification -> Executor handoff mechanically
  works, not as a proposal for Doctor's real prompt-types. Reused the existing, already-real
  `appsettings.json` model names (`qwen2.5:7b` for Classification, `hc-doctor-executor:latest` for
  Executor — these already exist as `HCClassification`/`HCDocExecutor` config sections) rather than
  inventing new ones.
- `HC.AI.MAPI.BL.Persona.PersonaModelResolutionBL` (`IPersonaModelResolutionBL`) —
  `ResolveClassification(roleId)` and `ResolveExecutor(roleId, promptTypeCode)`. Resolution only —
  doesn't run anything against Ollama itself, same "decide vs. execute" split `ILLMModelBL`
  already has.
- `PersonaModelResolutionController` — two GET endpoints purely to exercise/verify the mechanism:
  `/api/PersonaModelResolution/classification/{roleId}`,
  `/api/PersonaModelResolution/executor/{roleId}/{promptTypeCode}`
- Registered in `Program.cs`: mock provider as singleton (static in-memory data), resolution BL as
  scoped

## Deliberately not done
- **Not wired into `DoctorController.ProvidePrompt`** (the live, "Locked" Doctor chat endpoint).
  Doing so would mean deciding the fallback-behavior questions US012/US013's acceptance criteria
  explicitly flag as needing Product Owner sign-off (what happens on an unmapped classification
  result; error vs. persona-default when no Executor is configured) — silently wiring it in would
  answer those by default instead of by decision.
- **No real Classification LLM call.** Only the config-resolution step (which model + which
  prompt) is implemented. Actually calling Ollama with the classification prompt and parsing its
  output into a `PersonaPromptType.Code` is separate, larger work, and doing it against a single
  placeholder prompt-type wouldn't prove much yet.
- **No DB tables.** Still blocked on the schema doc's four open questions.

## Verification
- `dotnet build` on `HC.AI.MAPI.sln` — clean (0 errors; same 2 pre-existing unrelated nullable
  warnings in `BaseModel.cs` seen in every prior build this project)
- Live, running the API (`dotnet run`, port 5150):
  - `GET /api/PersonaModelResolution/classification/1` → `200 {"isResolved":true,"modelName":"qwen2.5:7b","provider":"Ollama",...}`
  - `GET /api/PersonaModelResolution/executor/1/General` → `200 {"isResolved":true,"modelName":"hc-doctor-executor:latest",...}`
  - `GET /api/PersonaModelResolution/executor/1/NotARealCode` → `404` (unknown code, correctly unresolved)
  - `GET /api/PersonaModelResolution/classification/2` (Patient, not seeded) → `404` (correctly unresolved)

## References
- [TASK017](../../scrum/tasks/TASK017_US011_US012_US013_persona_resolution_mock.md)
- [EPIC001](../../product_owner/epics/EPIC001_dynamic_persona_llm_prompt_resolution.md)
- [Schema proposal](../../architecture/design_patterns/persona_dynamic_llm_prompt_schema.md)
- [BACKLOG.md](../../product_owner/backlog/BACKLOG.md) — PB032
