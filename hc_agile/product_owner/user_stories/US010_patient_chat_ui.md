# US010 - Chat UI for Patient Persona (Angular, mock-first)

**As a** Patient
**I want to** ask about my own appointments, medications, and visit history through a chat window
**So that** I don't need a separate screen per question type, and I only ever see my own data

## Background
Follow-on from US008 (Doctor chat) and US009 (Auth). Architecture question — reuse the Doctor
chat window vs. build a separate one — was raised and answered before any code was written: a
Patient's chat needs are a genuine fork from a Doctor's (patient sees only their own record, no
cross-patient aggregate queries; different guardrails/system prompt would apply on a real
backend), so this is a **separate page**, not a persona-branch inside `ChatPage`. The
message-rendering UI (`MessageList`, `MessageItem`, `Composer`, `ChatRail`) was genuinely
persona-agnostic already, so those were promoted from `features/chat/components/` to
`shared/components/` and reused rather than duplicated.

## Backend status
**Update 2026-07-22 (US023/TASK020): real backend now wired.** `PatientChatMockService` has been
removed; `PatientChatPage` now calls the real `PatientChatService` →
`POST /api/Patient/provide-prompt`, mirroring Doctor's Module 1 pipeline exactly (own
`PatientController`/`PatientService`/`PatientPromptMapper`/`PatientSemanticProcess`/
`PatientPromptProvider`, resolving to its own `hc-patient-executor` Ollama model, not Doctor's).
See US023 for the follow-up story and its worklog for the live end-to-end verification.

## Acceptance Criteria
- [x] Separate `PatientChatPage` at `/patient-chat`, built on the shared presentational components
- [x] Mock data scoped to the patient's own record only (appointments, medications) — no
      cross-patient aggregate queries, reinforcing the actual guardrail distinction from Doctor
- [x] Home page's Patient action card now links to `/patient-chat` instead of showing a
      "not built yet" placeholder
- [x] Route guard applied (same `authGuard` as `/home` and `/chat`)
- [x] Logout available from the chat rail (same as Doctor chat)
- [x] Unit tests for the new page/service

## Explicitly out of scope (superseded, see Update above)
- ~~Real backend integration — no Patient API exists~~ — done, see US023

## Priority: Medium
## Status: Done (UI, mock-first) — real backend wiring tracked separately as US023 (Done)
## Sprint: Unscheduled — not yet assigned to a sprint plan
