# Persona-Based Model Selection — Wired End to End (2026-07-21)

Implements the remainder of BACKLOG PB034 (persona must be explicitly carried and branched on),
building on top of `PromptRequest.Persona`, `APIConstants.PatientPersonaName`/
`PatientExecutorPersonaName`, and `DoctorPromptMapper`'s `ModelKey` computation that had already
landed in `HC.AI.MAPI` (concurrent work, found already in place when picking this up — see
"What was already done" below).

## Background
Dev Angular Agent flagged a bug while reviewing a change to `PromptRequest`: `DoctorPromptMapper`
had a hardcoded `Persona = APIConstants.PatientExecutorPersonaName` on the **Doctor** endpoint's
own mapper — every Doctor request was being internally tagged as Patient. Not a live behavioral
bug at the time (`LLMModelBL` ignored `PromptItem.Persona`/`ModelKey` entirely and always resolved
`HCDocExecutor` regardless), but a landmine for whenever persona branching got wired downstream.
User asked to set persona correctly and coordinate with Dev Semantic Kernel (owner of this
pipeline) to complete it.

## What was already done (found in place, not authored in this worklog's session)
- `APIConstants.PatientPersonaName` ("Patient") and `PatientExecutorPersonaName`
  ("HCPatientExecutor") added
- `PromptItem.ModelKey` added — the resolved appsettings.json section name, separate from
  `Persona` (the label) itself
- `DoctorPromptMapper.ToPromptItem` fixed: `Persona = request.Persona` (no longer hardcoded), and
  `ModelKey` computed via `string.Equals(request.Persona, APIConstants.PatientPersonaName, ...)`
  → `PatientExecutorPersonaName`, else → `DoctorExecutorPersonaName`
- `appsettings.json` gained an `HCPatientExecutor` section (`hc-patient-executor:1.1`), alongside
  the existing `HCDocExecutor` (`hc-doctor-executor:latest`)
- Both models confirmed already pulled in Ollama (`hc-patient-executor:1.1`, and a newer
  `hc-patient-executor:1.2` also present — not switched to, out of scope here, just noted)

## What this session completed
The one piece still missing: **nothing downstream actually read `ModelKey`**.
`LLMModelBL.GetModelDetails` still called
`_llmOptionsFactory.GetLLMOptions(APIConstants.DoctorExecutorPersonaName)` — hardcoded, ignoring
the `ModelKey` the mapper now correctly computes. Fixed:

```csharp
// Before
var llmOptions = _llmOptionsFactory.GetLLMOptions(APIConstants.DoctorExecutorPersonaName);
// After
var llmOptions = _llmOptionsFactory.GetLLMOptions(model.PromptItem.ModelKey);
```

Also updated `ILLMModelBL`'s and `LLMModelBL`'s doc comments (they still described the old
hardcoded-Doctor-only behavior) to reflect that persona branching is now real, at the
appsettings-config level (not yet database-backed — that's still PB019/PB032/EPIC001).

Confirmed pipeline order is safe: `DoctorService.ProvidePromptAsync` calls
`_doctorPromptMapper.ToPromptItem(model)` before `_llmModelBL.GetModelDetails(model)`, so
`ModelKey` is always set by the time model resolution runs.

### Angular side
`doctor-chat.service.ts`'s `PromptRequest` interface had no `persona` field at all — added it,
sent as `'Doctor'` from `DEFAULT_GENERATION_CONTROLS`. Not strictly required for correctness today
(the mapper defaults to Doctor's `ModelKey` when `Persona` doesn't match Patient's), but PB034
asks that persona be *explicitly* carried on every request, not left implicit.

## Verification
- `dotnet build` on `HC.AI.MAPI.sln` — clean (had to stop the already-running `dotnet run`
  process first; its locked output DLLs caused an MSB3027 file-lock build failure — not a code
  problem, just the running process holding the files)
- `ng build` / `ng test` — clean, 58/58 passing, no spec changes needed
- Live, direct to `HC.AI.MAPI` (bypassing the Angular proxy, to isolate the backend fix):
  - `persona: "Doctor"` → `modelUsed: "hc-doctor-executor:latest"`
  - `persona: "Patient"` → `modelUsed: "hc-patient-executor:1.1"` — confirms the branch actually
    switches models, not just that Doctor still works
- Live, through the real Angular proxy path (`http://localhost:4200/api/Doctor/provide-prompt`)
  with the updated Angular request shape: `200`, `modelUsed: "hc-doctor-executor:latest"`

## Still not done (separate, larger scope)
- Database-backed persona/model config (PB019/PB032/EPIC001) — this fix is appsettings-based
  branching only, same shape as before, just no longer stuck on one persona
- No `PatientController`/live Patient endpoint exists yet — Patient persona is reachable through
  this same `DoctorController` endpoint if a caller sends `persona: "Patient"`, but nothing in
  the actual product UI does that (Patient chat is still fully mock, per PB024)
- Guardrail/query-DSL wiring (PB020) — untouched, unrelated to this fix

## References
- BACKLOG.md — PB034
- [TASK019](../../scrum/tasks/TASK019_PB034_persona_model_selection.md)
