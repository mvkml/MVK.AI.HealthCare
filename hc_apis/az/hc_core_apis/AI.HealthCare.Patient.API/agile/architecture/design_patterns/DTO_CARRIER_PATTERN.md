# Design Pattern — DTO / Carrier Model

## Owned by
Architect Agent

## Where Implemented
`AI.HealthCare.Patient.Models/<Domain>/<Domain>Item.cs`, one subfolder + namespace per domain (18 total).

## What It Is
A plain data-carrier class (no behavior) that moves data between layers, so EF Core entities never leak above the Repository layer, and the wire contract (once Controllers exist) can diverge from the storage shape if needed.

## How It Was Implemented (so far)
- Each domain has its own subfolder + namespace, e.g. `AI.HealthCare.Patient.Models.Patient` → `PatientItem`, `AI.HealthCare.Patient.Models.Allergy` → `AllergyItem`.
- `<Domain>Item` mirrors the EF entity's columns 1:1 for now (no fields hidden yet) — Repositories convert `Entity ↔ Item` internally.
- Foreign keys are carried as plain `Guid`/`long` id properties (e.g. `ProviderItem.OrganizationId`), not nested navigation objects — matches `AI.HR.Api`'s `UserItem.RoleId` convention. Keeps each `Item` shallow and serialization-safe.

## Not yet built (next, as BL/Controllers are added per domain)
- `<Domain>Request` — inbound payload shape for Create/Update endpoints (may exclude server-generated fields).
- `<Domain>Response : BaseModel` — outbound shape (may exclude internal-only fields, e.g. PII masking on `Patient.Ssn`/`Drivers`/`Passport` is an open decision).
- `<Domain>sModel : BaseModel` — carrier envelope for batch operations, matching `AI.HR.Api`'s `OcrDocumentsModel` convention (see `AI.HR.Api`'s own `design_patterns/OCR_DOCUMENT_PATTERNS.md`).
This three-part expansion (`Request`/`Item`/`Response`, plus envelope) happens per domain as its BL + Controller are built — not all at once.

## Why
- Keeps persistence concerns (EF entity shape, FK constraints) fully decoupled from what the API exposes.
- Matches `ai_hr`'s established convention: every domain gets its own subfolder + namespace with the `Request`/`Item`/`Response`/envelope carrier pattern.

---
*Defined by: Architect Agent | Date: 2026-07-15*
