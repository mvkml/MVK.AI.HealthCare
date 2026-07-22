# QA Wiki

Module-wise, version-wise archive of QA folder structure and test coverage. Each module below has
its own folder; each entry links to the current (latest) version file.

| Module | Current version | Covers |
|---|---|---|
| [`hc_qa_folder_structure/`](hc_qa_folder_structure/v2_2026-07-21.md) | v2 (2026-07-21) | The overall `hc_qa/` tree — the master flow chart |
| [`login/`](login/v1_2026-07-21.md) | v1 (2026-07-21) | Doctor/Patient login (`tests/login/login.spec.ts`) |
| [`signup/`](signup/v1_2026-07-21.md) | v1 (2026-07-21) | Doctor/Patient signup (`tests/signup/signup.spec.ts`) |
| [`admin-login/`](admin-login/v1_2026-07-21.md) | v1 (2026-07-21) | Admin login (`tests/admin-login/admin-login.spec.ts`) |
| [`identity-api/`](identity-api/v1_2026-07-21.md) | v1 (2026-07-21) | `HC.AI.Identity.Api` (`hc_qa/api/hc_ai_identity_api`) |
| [`ai-hc-api/`](ai-hc-api/v1_2026-07-21.md) | v1 (2026-07-21) | `AI.HealthCare.Patient.API` (`hc_qa/api/ai_hc_api`) |
| [`hc-ai-mapi/`](hc-ai-mapi/v1_2026-07-21.md) | v1 (2026-07-21) | `HC.AI.MAPI` persona -> model routing (`hc_qa/api/hc_ai_mapi`) |
| [`epics-features/`](epics-features/v1_2026-07-21.md) | v1 (2026-07-21) | Epic → Feature → User Story hierarchy snapshot |

## Adding a new version
When a module's coverage changes meaningfully (new test cases, execution results, folder
restructuring), add `v<N+1>_<YYYYMMDD>.md` to that module's folder and update the table above to
point at it — don't edit the old version file.
