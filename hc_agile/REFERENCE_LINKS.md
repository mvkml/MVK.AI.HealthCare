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

## `HC.AI.MAPI` — Healthcare AI Assistant (Doctor persona, .NET + Ollama)

Implementation of the design below, under `hc_ai_in/mapi` (supersedes the earlier `hc_chatbot/`
planning folder — that README/design docs still apply, the code just lives here now).

| What | Link |
|---|---|
| Base URL | `http://localhost:5150/api` |
| Swagger UI | `http://localhost:5150/swagger/index.html` |
| Start command | `cd hc_ai_in/mapi/HC.AI.MAPI/HC.AI.MAPI && dotnet run --urls http://localhost:5150` |
| Doctor persona — real LLM path | `GET http://localhost:5150/api/Doctor?message=hi` |
| Doctor persona — hardcoded path | `GET http://localhost:5150/api/Doctor/base-concept?message=hi` |
| Doctor persona — chat via prompt provider | `GET http://localhost:5150/api/Doctor/chat-response-by-prompt?message=hi` |
| Doctor persona — structured prompt (validated) | `POST http://localhost:5150/api/Doctor/provide-prompt` (body: `PromptRequest`) |
| Requires | Ollama running locally (`http://localhost:11434`), model `qwen2.5:7b` pulled |
| Source | [`hc_ai_in/mapi/HC.AI.MAPI/`](../hc_ai_in/mapi/HC.AI.MAPI/) |
| US007 / TASK006 | [`hc_agile/product_owner/user_stories/US007_healthcare_ai_assistant.md`](product_owner/user_stories/US007_healthcare_ai_assistant.md), [`hc_agile/scrum/tasks/TASK006_US007_doctor_controller_hello_slice.md`](scrum/tasks/TASK006_US007_doctor_controller_hello_slice.md) |
| Technical design document (Option A: fully custom) | [`hc_agile/architecture/design_patterns/healthcare_ai_assistant_mcp_ollama_design.md`](architecture/design_patterns/healthcare_ai_assistant_mcp_ollama_design.md) |
| Technical design document (Option B: ASP.NET Core + Semantic Kernel — not used; `HC.AI.MAPI` went the hand-rolled route instead) | [`hc_agile/architecture/design_patterns/healthcare_ai_assistant_semantic_kernel_design.md`](architecture/design_patterns/healthcare_ai_assistant_semantic_kernel_design.md) |
| Full chronological discussion log & field inventory | [`hc_agile/worklogs/learn/ai_mcp_ollama/2026-07-16_patient_ai_assistant_plan.md`](worklogs/learn/ai_mcp_ollama/2026-07-16_patient_ai_assistant_plan.md) |
| Service Layer / Agent Layer architecture discussion | [`hc_agile/worklogs/learn/ai_mcp_ollama/2026-07-17_service_layer_architecture_discussion.md`](worklogs/learn/ai_mcp_ollama/2026-07-17_service_layer_architecture_discussion.md) |
| Doctor Agent first slice worklog | [`hc_agile/worklogs/learn/ai_mcp_ollama/2026-07-17_doctor_agent_first_slice.md`](worklogs/learn/ai_mcp_ollama/2026-07-17_doctor_agent_first_slice.md) |
| Chosen Ollama model | `qwen2.5:7b` (pulled locally) |

## `aihcweb` — Angular chat UI (Doctor persona, US008)

| What | Link |
|---|---|
| Start command | `cd hc_ui/aihcweb && npm start` (serves on `http://localhost:4200`) |
| Design mockup | [`hc_ui/aihcweb/design/chat_mockup.html`](../hc_ui/aihcweb/design/chat_mockup.html) — static HTML reference, no backend wired |
| Angular CLI version | `@angular/cli@21.2.19` (pinned — `@angular/cli@latest`/v22 requires Node `^24.15.0`/`>=26.0.0`; local Node is `v24.12.0`) |
| Source | [`hc_ui/aihcweb/`](../hc_ui/aihcweb/) |
| US008 / worklog | [`hc_agile/product_owner/user_stories/US008_chat_ui_doctor_persona.md`](product_owner/user_stories/US008_chat_ui_doctor_persona.md), [`hc_agile/worklogs/dev_angular/20260717_225719_aihcweb_scaffold_and_chat_mockup.md`](worklogs/dev_angular/20260717_225719_aihcweb_scaffold_and_chat_mockup.md) |

## Reference docs

| What | Link |
|---|---|
| Durable Function design plan | [`hc_agile/architecture/design_patterns/durable_function_bulk_import_plan.md`](architecture/design_patterns/durable_function_bulk_import_plan.md) |
| Bulk CSV upload learning notes | [`hc_agile/worklogs/learn/bulk_data_import/2026-07-16_bulk_csv_upload_options.md`](worklogs/learn/bulk_data_import/2026-07-16_bulk_csv_upload_options.md) |
| Check-in policy (commit rules) | [`hc_agile/architecture/decisions/CHECKIN_POLICY.md`](architecture/decisions/CHECKIN_POLICY.md) |
