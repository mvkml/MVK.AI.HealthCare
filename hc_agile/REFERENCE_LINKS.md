# Reference Links

Central, living list of local URLs, endpoints, and key files used across the project — updated
as new services/endpoints are added, so we don't have to hunt through chat history to find them.

**Owner:** Scrum Master Agent owns this file and enforces that it stays current (same split as
`hc_agile/worklogs/`) — each dev agent updates its own section when it adds or changes a
service/endpoint.

## Patient API (`AI.HealthCare.Patient.API`)

| What | Link |
|---|---|
| Base URL | `http://localhost:5295/api` |
| Swagger UI | `http://localhost:5295/swagger/index.html` |
| Start command | `cd hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/AI.HealthCare.Patient.API && dotnet run` |
| Source | [`hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/`](hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/) |
| Synthea CSV files | [`hc_apis/.../documents/patient_details/synthea/`](hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/) |

## `HC.AI.Identity.Api` — Authentication (US009 backend, PB023)

Renamed 2026-07-19 from `AI.HR.Api` once PB023 unblocked. Schema (`Users`, `Roles`,
`OcrDocuments`) merged into `AI_HealthCarePatient`.

| What | Link |
|---|---|
| Base URL | `http://localhost:5008/api` |
| Swagger UI | `http://localhost:5008/swagger/index.html` |
| Start command | `cd hc_apis/az/hc_core_apis/HC.AI.Identity.Api/HC.AI.Identity.Api && dotnet run --urls http://localhost:5008` |
| Endpoints | `POST /users/signup`, `POST /users/login`, `POST /users/forgot-password`, `POST /users/reset-password`, `GET /users/roles` |
| Source | [`hc_apis/az/hc_core_apis/HC.AI.Identity.Api/`](../hc_apis/az/hc_core_apis/HC.AI.Identity.Api/) |
| Playwright tests | [`hc_qa/api/hc_ai_identity_api/`](hc_qa/api/hc_ai_identity_api/) — `npm test` (19 tests) |
| TASK013 (wiring Angular to this API) | [`hc_agile/scrum/tasks/TASK013_US009_wire_real_auth_backend.md`](scrum/tasks/TASK013_US009_wire_real_auth_backend.md) |

## QA test suites (`hc_qa/`)

One folder per application, under `hc_qa/api/<project>/` (backend APIs) and `hc_qa/web/<project>/`
(web/mobile apps) — see `hc_agile/team/dev_qa_agent.md`.

| Folder | Covers | Status |
|---|---|---|
| [`hc_qa/api/ai_hc_api/`](hc_qa/api/ai_hc_api/) | `AI.HealthCare.Patient.API` | Tests + CSV-upsert demos (`npm run demo:<entity>-upsert`, see [`package.json`](hc_qa/api/ai_hc_api/package.json)) |
| [`hc_qa/api/hc_ai_identity_api/`](hc_qa/api/hc_ai_identity_api/) | `HC.AI.Identity.Api` | 19 tests |
| [`hc_qa/api/hc_ai_mapi/`](hc_qa/api/hc_ai_mapi/) | `HC.AI.MAPI` | Skeleton, no tests yet |
| [`hc_qa/api/hc_fapi/`](hc_qa/api/hc_fapi/) | `hc_fapps` (FastAPI) | Skeleton, no tests yet |
| [`hc_qa/web/aihcweb/`](hc_qa/web/aihcweb/) | `aihcweb` | `demo1`/`demo2` (login → Doctor/Patient chat) — see [`demos/README.md`](hc_qa/web/aihcweb/demos/README.md); no test specs yet |
| [`hc_qa/web/mvkhcapp/`](hc_qa/web/mvkhcapp/) | `mvkhcapp` | Skeleton, no tests yet |

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

## Ollama Custom Models (`hc_llms`)

Persona-specific Ollama models built on top of the base `qwen2.5:7b` model, under
`hc_llm/llm/ollama/custom_models/hc_llms/` (one folder per persona, split into `classification`
and `executor` roles per the multi-agent pipeline design). Setup steps and template Modelfile
walkthrough: [`hc_llm/llm/ollama/custom_models/hc_llms/guide/custom_model_setup_guide.md`](../hc_llm/llm/ollama/custom_models/hc_llms/guide/custom_model_setup_guide.md).

**Versioning**: build with an explicit tag (`ollama create hc-doctor-executor:v1 -f <Modelfile>`)
rather than the untagged default (silently resolves to `:latest`, overwritten on next build with
no history) — bump the tag each time the Modelfile changes, reference the full `name:tag`
everywhere the model is used.

| Model | Persona/Role | Base | `ollama create` run? | Modelfile | Notes |
|---|---|---|---|---|---|
| `qwen2.5:7b` | — (base model) | — | Pulled | — | Used directly by `HC.AI.MAPI` today (see above) |
| `hc-doctor-executor` | Doctor / Executor | `qwen2.5:7b` | **Yes — created** | [`hc_llms/doctor/executor/Modelfile`](../hc_llm/llm/ollama/custom_models/hc_llms/doctor/executor/Modelfile) | "Dr. Ajmath's AI Clinical Assistant" persona; built **untagged** (`:latest`), predates the versioning convention — retag to `:v1` next update; not yet wired into `appsettings.json` |
| `hc-doctor-classifier` | Doctor / Classification | `qwen2.5:7b` | No | Not yet written | Intent/routing model — planned, build as `:v1` |
| `hc-patient-executor` | Patient / Executor | `qwen2.5:7b` | No | Not yet written | Planned, build as `:v1` |
| `hc-patient-classifier` | Patient / Classification | `qwen2.5:7b` | No | Not yet written | Planned, build as `:v1`; folder is `patient/clasification` (typo, not yet fixed) |

General Ollama background (AI/ML primer, Ollama history/advantages, configuration, Modelfile
guardrails and customization) — demo/PPT source content, kept separate from the above
persona-specific build: [`hc_agile/hc_llm/ollama/`](hc_llm/ollama/) (files `01`–`05`).

Same content is also published to the Azure DevOps project Wiki under
**Large Language Models (LLM) → Ollama** (see the "Azure DevOps" section below for the org/PAT
reference).

## `aihcweb` — Angular chat UI (Doctor persona, US008)

| What | Link |
|---|---|
| Start command | `cd hc_ui/aihcweb && npm start` (serves on `http://localhost:4200`) |
| Design mockup | [`hc_ui/aihcweb/design/chat_mockup.html`](../hc_ui/aihcweb/design/chat_mockup.html) — static HTML reference, no backend wired |
| Angular CLI version | `@angular/cli@21.2.19` (pinned — `@angular/cli@latest`/v22 requires Node `^24.15.0`/`>=26.0.0`; local Node is `v24.12.0`) |
| Source | [`hc_ui/aihcweb/`](../hc_ui/aihcweb/) |
| US008 / worklog | [`hc_agile/product_owner/user_stories/US008_chat_ui_doctor_persona.md`](product_owner/user_stories/US008_chat_ui_doctor_persona.md), [`hc_agile/worklogs/dev_angular/20260717_225719_aihcweb_scaffold_and_chat_mockup.md`](worklogs/dev_angular/20260717_225719_aihcweb_scaffold_and_chat_mockup.md) |

## Azure DevOps — Boards/Work Items (process setup, not CI/CD)

| What | Link |
|---|---|
| Org/Project | `https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care` |
| Epic #1 — "Azure DevOps Environment Setup" | https://dev.azure.com/mvishnukiran05/a9d525a0-0840-4b29-8475-fbb3d23acb9d/_workitems/edit/1 |
| Issue #2 — "Set up the environment" (child of Epic #1) | https://dev.azure.com/mvishnukiran05/a9d525a0-0840-4b29-8475-fbb3d23acb9d/_workitems/edit/2 |
| PAT setup/storage guide | [`hc_devops/PAT_SETUP_GUIDE.md`](../hc_devops/PAT_SETUP_GUIDE.md) |
| Worklog | [`hc_agile/worklogs/dev_devops/20260720_125630_azure_devops_environment_setup.md`](worklogs/dev_devops/20260720_125630_azure_devops_environment_setup.md) |

## Reference docs

| What | Link |
|---|---|
| Durable Function design plan | [`hc_agile/architecture/design_patterns/durable_function_bulk_import_plan.md`](architecture/design_patterns/durable_function_bulk_import_plan.md) |
| Bulk CSV upload learning notes | [`hc_agile/worklogs/learn/bulk_data_import/2026-07-16_bulk_csv_upload_options.md`](worklogs/learn/bulk_data_import/2026-07-16_bulk_csv_upload_options.md) |
| Check-in policy (commit rules) | [`hc_agile/architecture/decisions/CHECKIN_POLICY.md`](architecture/decisions/CHECKIN_POLICY.md) |
