# QA — HC.AI.Identity.Api

Playwright API test suite for `HC.AI.Identity.Api` (Dev .NET Agent's authentication API — signup,
login, forgot-password, reset-password, roles). Sibling to `hc_qa/api/ai_hc_api/`.

Renamed 2026-07-19 from `AI.HR.Api` once PB023 (real auth backend) unblocked — no longer
reference-only. Schema (`Users`, `Roles`, `OcrDocuments`) merged into `AI_HealthCarePatient`, the
same database `AI.HealthCare.Patient.API` uses. Roles simplified to `Doctor` / `Patient`.

**Status:** 19 tests written and passing against a live run of the API (`tests/users.spec.ts`),
covering all 5 endpoints below (happy path + validation errors). Dev port: `http://localhost:5008`.

## Endpoints (from `UsersController`)
- `POST /api/users/signup`
- `POST /api/users/login`
- `POST /api/users/forgot-password`
- `POST /api/users/reset-password`
- `GET /api/users/roles`

## Layout (once populated)
```
hc_ai_identity_api/
├── tests/       ← Playwright specs
└── fixtures/    ← shared test data
```
