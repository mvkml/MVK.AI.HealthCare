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

## `hc_chatbot` — Healthcare AI assistant (MCP + Ollama)

| What | Link |
|---|---|
| Status | Planning stage — no implementation code yet |
| Project overview | [`hc_chatbot/README.md`](../hc_chatbot/README.md) |
| Technical design document (Option A: fully custom) | [`hc_agile/architecture/design_patterns/healthcare_ai_assistant_mcp_ollama_design.md`](architecture/design_patterns/healthcare_ai_assistant_mcp_ollama_design.md) |
| Technical design document (Option B: ASP.NET Core + Semantic Kernel) | [`hc_agile/architecture/design_patterns/healthcare_ai_assistant_semantic_kernel_design.md`](architecture/design_patterns/healthcare_ai_assistant_semantic_kernel_design.md) |
| Full chronological discussion log & field inventory | [`hc_agile/worklogs/learn/ai_mcp_ollama/2026-07-16_patient_ai_assistant_plan.md`](worklogs/learn/ai_mcp_ollama/2026-07-16_patient_ai_assistant_plan.md) |
| Chosen Ollama model | `qwen2.5:7b` (pulled locally) |

## Reference docs

| What | Link |
|---|---|
| Durable Function design plan | [`hc_agile/architecture/design_patterns/durable_function_bulk_import_plan.md`](architecture/design_patterns/durable_function_bulk_import_plan.md) |
| Bulk CSV upload learning notes | [`hc_agile/worklogs/learn/bulk_data_import/2026-07-16_bulk_csv_upload_options.md`](worklogs/learn/bulk_data_import/2026-07-16_bulk_csv_upload_options.md) |
| Check-in policy (commit rules) | [`hc_agile/architecture/decisions/CHECKIN_POLICY.md`](architecture/decisions/CHECKIN_POLICY.md) |
