# TASK017 - Implement EPIC001 resolution mechanism with mock config models

**US:** US011, US012, US013
**Status:** Done (mock) — 2026-07-20
**Assigned:** Dev .NET Agent (HC.AI.MAPI)

## Why this exists
US011/US012/US013 (EPIC001, PB032) were all blocked on open design questions in
[persona_dynamic_llm_prompt_schema.md](../../architecture/design_patterns/persona_dynamic_llm_prompt_schema.md)
before real DB tables could be created. User asked to implement the mechanism with mock models
instead of waiting on that sign-off — same mock-first pattern used throughout this project.

## What was built (in `HC.AI.MAPI`)
- `HC.AI.MAPI.Models.Persona` — `PersonaPromptType`, `PersonaLlmOption`, `PersonaPromptRecord`,
  `ModelRole` (Classification/Executor constants), `PersonaModelResolutionResult` — C# shapes
  mirroring the three proposed tables
- `HC.AI.MAPI.BL.Persona.PersonaLlmConfigMockProvider` — in-memory data standing in for the DB.
  Doctor (RoleId 1) only, one prompt-type (`"General"`) explicitly labeled as a placeholder — the
  schema doc says real prompt-type values are "not guessing at these," so this mock doesn't
  invent business-meaningful ones either, just enough to prove the mechanism
- `HC.AI.MAPI.BL.Persona.PersonaModelResolutionBL` — implements US012 (resolve Classification
  config for a persona) and US013 (resolve Executor config for a persona + prompt-type code).
  Unresolvable lookups (unknown code, unseeded persona) return `IsResolved = false` rather than
  guessing a fallback — both stories' ACs flag fallback behavior as needing Product Owner
  sign-off, not assumed here
- `PersonaModelResolutionController` — demo-only endpoints (`GET
  /api/PersonaModelResolution/classification/{roleId}`,
  `GET /api/PersonaModelResolution/executor/{roleId}/{promptTypeCode}`) to exercise the mechanism
  live without touching the locked Doctor `provide-prompt` path

## Deliberately NOT done
- **Not wired into the live `DoctorController.ProvidePrompt` flow.** Doing so would mean actually
  deciding the open fallback-behavior questions (what happens on an unmapped classification
  result, or a persona+type with no configured Executor) — both ACs explicitly flag those as
  Product Owner decisions. Wiring it in silently would answer those questions by default instead
  of by decision.
- **No real Ollama classification call chained in.** The mock provider only proves the *config
  resolution* step (which model/prompt to use); actually running the Classification model against
  a live message and parsing its output into a `PersonaPromptType.Code` is separate work, and
  doing that against a single placeholder prompt-type wouldn't prove much yet.
- No DB tables — still blocked on the schema doc's open questions (single- vs multi-role-per-user,
  real prompt-type values, fallback semantics, `Roles` naming).

## Verification
- `dotnet build` — clean (0 errors, same 2 pre-existing unrelated nullable warnings)
- Live, via the running API:
  - `GET /api/PersonaModelResolution/classification/1` → 200, resolves to `qwen2.5:7b` / the mock
    Classification prompt
  - `GET /api/PersonaModelResolution/executor/1/General` → 200, resolves to
    `hc-doctor-executor:latest` / the mock Executor prompt
  - `GET /api/PersonaModelResolution/executor/1/NotARealCode` → 404 (unresolved, as expected)
  - `GET /api/PersonaModelResolution/classification/2` (Patient, not seeded) → 404 (unresolved, as
    expected)

## Backlog reference
`BACKLOG.md` PB032.
