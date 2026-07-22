# TASK020 - Wire Patient chat to a real HC.AI.MAPI backend

**US:** US023
**Status:** Done — 2026-07-22
**Assigned:** Dev Semantic Kernel Agent (backend pipeline) + Dev Angular Agent (UI wiring)

## Why this was picked up now
User asked directly whether the Patient-side equivalent of Doctor's Module 1 existed, then
explicitly authorized implementing it end-to-end ("Implement patient patient chart... implement
the functionality accordingly").

## Scope — backend (`HC.AI.MAPI`)
Mirrored Doctor's Module 1 layer-for-layer, new Patient-specific classes at each layer (not a
persona-branch inside the Doctor classes):
- `Controllers/PatientController.cs` — `POST /api/Patient/provide-prompt`, same validation/shape
  as `DoctorController`
- `Services/PatientService.cs` / `IPatientService.cs` — `Mapper -> LLMModelBL -> SemanticProcess`
  orchestration only (`ProvidePromptAsync`; no legacy demo methods, since Patient has none to
  mirror)
- `Services/Mapping/PatientPromptMapper.cs` / `IPatientPromptMapper.cs` — **hardcodes**
  `Persona`/`ModelKey` server-side rather than trusting `request.Persona`, deliberately, per the
  lesson from the original `DoctorPromptMapper` bug (US021)
- `AL/PatientSemanticProcess.cs` / `IPatientSemanticProcess.cs` — injects the shared
  `ISemanticProcessService` + new `IPatientPromptProvider`
- `Prompt/Patient/PatientPromptProvider.cs` / `IPatientPromptProvider.cs` — Patient-safe system
  prompt (plain language, no diagnosing/prescribing, defers to a real doctor)
- `Program.cs` — registered all four new services (`IPatientService`, `IPatientPromptProvider`,
  `IPatientSemanticProcess`, `IPatientPromptMapper`)
- Reused existing config: `appsettings.json`'s `HCPatientExecutor` section (added under
  TASK019/PB034) resolves to Ollama's `hc-patient-executor:1.1`

## Scope — Angular (`aihcweb`)
- Added `features/patient-chat/data/patient-chat.service.ts` (`PatientChatService`), mirroring
  `DoctorChatService`'s request/response shape 1:1 (`PromptRequest`/`PromptResponse`,
  `persona: 'Patient'` hardcoded client-side too — same intentional-hardcode reasoning as
  `doctor-chat.service.ts`, since `PatientChatPage` is only reachable by a logged-in Patient)
- `patient-chat-page.ts` — swapped `PatientChatMockService` → `PatientChatService`, added
  `errorMessage` signal and the same three-way error handling as `ChatPage` (network failure /
  400 validation array / generic HTTP error)
- `patient-chat-page.html` — badge changed from "Mock demo — no Patient backend yet" to
  "LLM only — not database-grounded yet" (matches Doctor's), added the `error-banner` block
- Deleted `patient-chat-mock.service.ts` and its spec (fully superseded, no remaining references)
- Rewrote `patient-chat-page.spec.ts` against `HttpTestingController` (was asserting against the
  mock's fixed-delay canned replies) — covers: real API call + reply rendering, network-failure
  error banner, 400-validation error banner, persona rail, logout

## Verification
- `npx ng test --watch=false`: 15 test files / 57 tests passing (includes the rewritten Patient
  suite)
- Live round trip through the actual Angular dev-server proxy (`localhost:4200/api/Patient/...`,
  not a direct backend call): real generated reply, `modelUsed: "hc-patient-executor:1.1"`,
  `isSuccess: true` — confirms the Patient persona resolves to its own model end-to-end, not
  Doctor's, and that the dev proxy config (already pointing `/api` at `HC.AI.MAPI`) needed no
  changes

## Backlog reference
`BACKLOG.md` PB036.
