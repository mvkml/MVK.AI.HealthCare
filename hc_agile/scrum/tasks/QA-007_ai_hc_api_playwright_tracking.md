# QA-007 - Track ai_hc_api Playwright suite (backfill)

**US:** US022
**Status:** Written (pre-existing) — not yet executed this session

## Description
`hc_qa/api/ai_hc_api/tests/patients.spec.ts` (17 tests: CRUD + CSV import + CSV import/upsert
against `AI.HealthCare.Patient.API`) existed before this audit but had no QA-xxx task file tracking
it — a genuine gap, since it's QA's own Playwright suite. This task backfills that tracking.

## Coverage
17 tests across 3 `test.describe` blocks: CRUD (create/read/update/delete + PII masking), CSV
import (insert, malformed-row handling, file-type/empty-file rejection), CSV import/upsert
(insert-if-new, update-in-place without duplicating).

## Next step
Not yet executed this session — needs `AI.HealthCare.Patient.API` running
(`cd hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/AI.HealthCare.Patient.API && dotnet run`,
base URL `http://localhost:5295/api`) before `npx playwright test` can run.

## References
- [US022](../../product_owner/user_stories/US022_qa_test_coverage_inventory.md)
