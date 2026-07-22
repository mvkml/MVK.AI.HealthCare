# Dev Semantic Kernel Agent — Work Log
## Date: 2026-07-22
## Subject: Real Patient chat backend pipeline (US023/TASK020)

## What Happened
User asked whether Patient had an executor equivalent to Doctor's, then explicitly authorized
building it, communicating with QA where needed.

Built a full parallel pipeline in `HC.AI.MAPI`, mirroring Doctor's Module 1
(`DoctorController -> DoctorService -> DoctorPromptMapper -> LLMModelBL -> DoctorSemanticProcess
-> DoctorPromptProvider`) with Patient-specific classes at every layer — not a persona-branch
inside the existing Doctor classes, consistent with how US010 treated the UI side (separate page,
not a branch inside `ChatPage`, because the guardrails/system prompt genuinely differ per persona).

## Key design decision
`PatientPromptMapper.ToPromptItem` **hardcodes** `Persona = APIConstants.PatientPersonaName` and
`ModelKey = APIConstants.PatientExecutorPersonaName` — it does not read `request.Persona` from the
client at all. This is deliberate: the original `DoctorPromptMapper` bug (found and fixed under
TASK019/PB034) was exactly a client-supplied field silently picking the wrong model. An endpoint
should decide its own persona from which endpoint was hit, not trust the caller. Doctor's mapper
was left reading `request.Persona` (already fixed to branch correctly) since changing that pattern
wasn't in scope here; Patient's new mapper was written hardcoded from the start so it can't regress
the same way.

## Config reused, not duplicated
`appsettings.json`'s `HCPatientExecutor` section and the `hc-patient-executor:1.1` Ollama model
already existed from TASK019/PB034 — no new config needed, only new C# classes to route to it.

## Build/verify
- `dotnet build`: 0 warnings, 0 errors (after stopping the locked running `dotnet run` process —
  MSB3027 file-lock, same recurring issue as prior sessions)
- Direct smoke test: `POST /api/Patient/provide-prompt` with an empty message → 400 (validation
  reachable, not a 404 stub)
- Full live round trip (message with real content) → `hc-patient-executor:1.1`, real generated
  content, patient-safe tone (no diagnosis/prescription, defers to a real doctor) — confirms
  `PatientPromptProvider`'s system prompt behaves as written

## QA
Did not file a new QA task — `QA-010`'s persona-routing coverage already exercises the model
selection mechanism this pipeline relies on (`LLMModelBL`/`ModelKey`), and this task only adds a
new caller of that same mechanism. Flagging to QA that a Patient-specific Playwright suite (chat
round trip, not just persona routing) is not yet covered — worth its own QA task if the user wants
Patient chat tested the way Doctor's was.

## References
- [US023](../../product_owner/user_stories/US023_wire_real_patient_chat_backend.md)
- [TASK020](../../scrum/tasks/TASK020_US023_wire_real_patient_chat_backend.md)
- [TASK019](../../scrum/tasks/TASK019_PB034_persona_model_selection.md) — prior persona-routing fix this builds on
