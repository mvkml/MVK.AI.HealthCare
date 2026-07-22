# US023 - Wire Patient Chat to a Real Backend (mirrors Doctor's Module 1)

**As a** Patient
**I want to** my chat messages to reach a real, Patient-scoped AI assistant instead of a canned
mock reply
**So that** the Patient chat experience is functionally equivalent to Doctor's already-live
Module 1, not a permanent UI-only demo

## Background
Follow-on from US010 (Patient chat UI, mock-first) and PB034/US021 (persona-based model
selection). US010 explicitly deferred real backend integration; this story closes that gap the
same way TASK009/US008 closed it for Doctor.

## Acceptance Criteria
- [x] `HC.AI.MAPI` gets a full Patient pipeline mirroring Doctor's Module 1: `PatientController`
      (`POST /api/Patient/provide-prompt`) → `PatientService` → `PatientPromptMapper` →
      `LLMModelBL` → `PatientSemanticProcess` → `PatientPromptProvider`
- [x] `PatientPromptMapper` **hardcodes** persona/`ModelKey` server-side (does not trust a
      client-supplied `Persona`) — same lesson learned from the original `DoctorPromptMapper` bug
      (see US021) — so the Patient endpoint resolves to its own `hc-patient-executor` Ollama model,
      never Doctor's, regardless of what the caller sends
- [x] `PatientPromptProvider`'s system prompt is Patient-safe: plain language, no
      diagnosing/prescribing, encourages following up with a real doctor
- [x] Angular: `PatientChatMockService` removed; `PatientChatPage` calls a real
      `PatientChatService` (`POST /api/Patient/provide-prompt`), mirroring `DoctorChatService`
      exactly (same request/response shape, same error handling for network failure vs. 400
      validation vs. success)
- [x] `patient-chat-page.html` badge/copy updated to match Doctor's real-backend page (no longer
      says "Mock demo")
- [x] Unit tests updated: `patient-chat-page.spec.ts` rewritten against `HttpTestingController`
      (was asserting against the mock's fixed 600ms-delayed canned replies)
- [x] Live end-to-end verification through the actual Angular dev-server proxy (not just a direct
      API call), confirming `modelUsed: "hc-patient-executor:1.1"` and a real generated reply

## Priority: High
## Status: Done — 2026-07-22
## Sprint: Unscheduled
## Worklogs:
- [20260722_113000_patient_chat_real_backend.md](../../worklogs/dev_semantic_kernel/20260722_113000_patient_chat_real_backend.md) — backend pipeline (Dev Semantic Kernel Agent)
- [20260722_113000_patient_chat_wired_to_real_backend.md](../../worklogs/dev_angular/20260722_113000_patient_chat_wired_to_real_backend.md) — Angular wiring (Dev Angular Agent)
