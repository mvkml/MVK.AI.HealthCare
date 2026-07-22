# QA-006 - Backend unit test coverage audit

**US:** US022
**Status:** Executed — audit complete, coverage gap flagged

## Description
QA's responsibility per `hc_agile/team/dev_qa_agent.md` is to validate that unit test coverage
exists (Dev agents write the tests themselves) — this task is that validation pass across every
xUnit test project in the repo, none of which had been checked or tracked by QA before now.

## Result

| Project | Tests | Coverage | Status |
|---|---|---|---|
| `AI.HealthCare.Patient.API.Tests` — `Patient/PatientValidationServiceTests.cs` | 7 `[Fact]` | Patient validation logic | OK |
| `HC.AI.Identity.Api.Tests` — `Security/PasswordHasherTests.cs` | 1 `[Fact]` | Password hashing only | Thin — login/signup/roles BL has no xUnit coverage (covered instead by QA's own Playwright suite, `hc_qa/api/hc_ai_identity_api`) |
| `HC.AI.Admin.Api.Tests` — `UnitTest1.cs` | 1 (scaffold `Test1`, empty) | None | **Gap** — no real test exists yet, just the default template |
| `df_chunk_file.Tests` — `ApiUploadClientTests.cs` / `FileChunkingServiceTests.cs` / `ImportResultAggregatorTests.cs` | 3 + 5 + 3 = 11 `[Fact]` | Bulk-import chunking/aggregation | OK |

## Flag for Product Owner / Dev .NET Agent
`HC.AI.Admin.Api.Tests` has zero real coverage (default scaffold only). Not urgent — `HC.AI.Admin.API`
itself is scaffold-only per US016/TASK016 — but worth a backlog line once real Admin API endpoints
exist, so the test project doesn't stay a placeholder indefinitely.

## References
- [US022](../../product_owner/user_stories/US022_qa_test_coverage_inventory.md)
