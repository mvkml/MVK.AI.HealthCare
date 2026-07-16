# Reference Links

Central, living list of local URLs, endpoints, and key files used across the project — updated
as new services/endpoints are added, so we don't have to hunt through chat history to find them.

## Patient API (`AI.HealthCare.Patient.API`)

| What | Link |
|---|---|
| Base URL | `http://localhost:5295/api` |
| Swagger UI | `http://localhost:5295/swagger/index.html` |
| Start command | `cd hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/AI.HealthCare.Patient.API && dotnet run` |
| Source | [`hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/`](hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/) |
| Synthea CSV files | [`hc_apis/.../documents/patient_details/synthea/`](hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/) |

## Playwright API tests & demos (`hc_qa/playwright_api`)

| What | Link |
|---|---|
| Source | [`hc_qa/playwright_api/`](hc_qa/playwright_api/) |
| Test suite | `npm test` (from `hc_qa/playwright_api`) |
| Per-entity CSV-upsert demos | `npm run demo:<entity>-upsert` (e.g. `demo:patient-upsert`) — see [`package.json`](hc_qa/playwright_api/package.json) for the full list |

## `df_chunk_file` — Durable Function bulk-import client

| What | Link |
|---|---|
| Start HTTP trigger | `http://localhost:7131/api/StartBulkImport` (POST) |
| Swagger UI | `http://localhost:7131/api/swagger/ui` |
| OpenAPI spec | `http://localhost:7131/api/openapi/v3.json` |
| Start command | `cd hc_apis/az/hc_fapps/df/df_chunk_file/df_chunk_file && func start --port 7131` |
| Requires | Azurite storage emulator running (`azurite --skipApiVersionCheck`) |
| Sample request body | [`Files/BulkImportRequest.json`](hc_apis/az/hc_fapps/df/df_chunk_file/df_chunk_file/Files/BulkImportRequest.json) |
| Source | [`hc_apis/az/hc_fapps/df/df_chunk_file/`](hc_apis/az/hc_fapps/df/df_chunk_file/) |

## Reference docs

| What | Link |
|---|---|
| Durable Function design plan | [`hc_agile/architecture/design_patterns/durable_function_bulk_import_plan.md`](architecture/design_patterns/durable_function_bulk_import_plan.md) |
| Bulk CSV upload learning notes | [`hc_agile/worklogs/learn/bulk_data_import/2026-07-16_bulk_csv_upload_options.md`](worklogs/learn/bulk_data_import/2026-07-16_bulk_csv_upload_options.md) |
| Check-in policy (commit rules) | [`hc_agile/architecture/decisions/CHECKIN_POLICY.md`](architecture/decisions/CHECKIN_POLICY.md) |
