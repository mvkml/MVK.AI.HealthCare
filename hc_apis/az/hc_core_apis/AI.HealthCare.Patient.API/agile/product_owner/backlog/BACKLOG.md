# AI.HealthCare.Patient.API — Backlog

**Status:** Core vertical build complete. **All 18 domains** now have fully working vertical slices (Entity → Mapper → Repository → BL → Controller → Swagger), live-tested: `Patients`, `Organizations`, `Providers`, `Payers`, `PayerTransitions`, `Encounters`, `Conditions`, `Allergies`, `Medications`, `Careplans`, `Procedures`, `Immunizations`, `Observations`, `Devices`, `Supplies`, `ImagingStudies`, `Claims`, `ClaimTransactions`. The full CRUD REST API is ready to expose as an OpenAPI tool spec for AI agent consumption. Remaining work is all backlog items below — no domain is left unbuilt.

## Backlog — Patients module (deferred items, not blocking further work)
| # | Item | Notes |
|---|---|---|
| 1 | `PatientBLTests` | Needs Moq added to `AI.HealthCare.Patient.API.Tests` — `PatientBL` depends on `IPatientRepository` |
| 2 | Controller-level tests for `PatientsController` | No tests yet for the HTTP layer (integration-style, hitting real endpoints) |
| 3 | CSV data import (`patients.csv` → `Patients` table) | Table is currently empty — Synthea rows never loaded |
| 4 | `GetAll` PII exposure | `GetAll` still returns raw unmasked `PatientItem`, inconsistent with the masked single-item endpoints (`includePii` flag only applies to Create/GetById/Update) |
| 5 | Auth (authentication/authorization) | No auth on any `PatientsController` endpoint |
| 6 | `patient-api-dev-frontend` agent + UI | Not created — nothing to consume the API yet |

## Backlog — Project-wide
| # | Item | Notes |
|---|---|---|
| 7 | ~~BL + Controller for remaining domains~~ | **Done.** All 18 domains complete as of 2026-07-15. |
| 8 | ~~Per-domain `<Domain>Mapper` rollout~~ | **Done.** All 18 domains have an extracted `<Domain>Mapper`, built one-by-one alongside each vertical slice rather than as a separate horizontal pass. |
| 9 | Normalize `Allergies`/`Claims` repeated columns | Schema decision flagged twice in `SQL_DESIGN_IMPLEMENTED.md`, still open |
| 10 | `Providers.OrganizationId` FK still `Cascade` not `Restrict` | Minor schema inconsistency, flagged, not fixed |
| 11 | `GetAll` scoped-query gaps | Only patient-linked domains got a `GetByPatientId` (or `GetByClaimId` for `ClaimTransactions`) route; a bare `GetAll` still returns the entire table for every domain with no filtering/paging — fine for a demo dataset, will need pagination before any real data volume |
| 12 | DevOps agent + CI/CD | Deferred — no deployment target decided |
| 13 | QA agent | Not requested yet |
| 14 | `Common` layer (Layer #4) | Deliberately deferred until an actual shared utility is needed |
| 15 | Per-module Technical Design Doc + Functional Design Doc + Architecture text-flow | Owned by Architect Agent. Deliberately deferred (see decision below) — one set of docs per module, capturing only what's domain-specific (deviations from `MODULE_TEXT_FLOW.md`'s generic flow: `long` vs `Guid` keys, required non-nullable `Date` fields, `Claims`/`ClaimTransactions`' larger FK surface). Not started yet — now that all 18 vertical slices are built, this is the next logical piece of work if the user wants it. |
| 16 | CSV data import for all 18 tables | All tables still empty — no Synthea rows loaded into any domain |
| 17 | Auth (authentication/authorization) | No auth on any Controller across all 18 domains |
| 18 | `patient-api-dev-frontend` agent + UI | Not created — nothing to consume the API yet beyond Swagger UI and the planned agent-tool integration |
| 19 | Controller-level / BL-level tests beyond `Patients` | Only `PatientValidationServiceTests` exist (7 tests); no BL or Controller tests for any of the other 17 domains, and no Moq-based mocking set up yet |

### Decision: design-doc timing (2026-07-15)
Discussed with the user whether to write full functional + technical design docs per module upfront (before implementation) or defer them. Decision: **defer, implement first.** The architecture is fixed and documented once, generically (`MODULE_TEXT_FLOW.md`); per-module docs would mostly repeat that same diagram with no new decisions to record. The as-built worklog entry per module (e.g. `allergies_vertical_slice.md`) already captures the technical record and any domain-specific deviation. Item #15 above formalizes this as a backlog item so it isn't forgotten.

## Next Steps
**Vertical build complete — all 18 domains done (2026-07-15).** No domain-specific implementation work remains; remaining work is entirely the backlog items above (design docs, data import, auth, tests, frontend). Awaiting direction from the user on which backlog item to tackle next, or whether to proceed with exposing the API as an OpenAPI tool definition for the AI agent integration discussed earlier.
