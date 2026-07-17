# mapi/fapi scaffolding + Ollama LLM layer (2026-07-17)

## Where the project folders landed

`hc_ai_in/{mai,fai}` (the multi-project `mai.Api/mai.Orchestration/mai.Plugins/mai.Models`
solution scaffolded on 2026-07-16) was replaced with two flat, single-project scaffolds created
directly by Vishnu:

- `hc_ai_in/mapi/HC.AI.MAPI/HC.AI.MAPI` — plain ASP.NET Core Web API (`dotnet new webapi`,
  Swashbuckle only, no Semantic Kernel yet)
- `hc_ai_in/fapi/HC.AI.FAPI` — plain FastAPI app (`main.py` + `models/` + `routers/`, fastapi +
  uvicorn only, no LangChain/LangGraph yet)

Decision: focus on **`HC.AI.MAPI` (.NET) first**, come back to `HC.AI.FAPI` later.

## Folder structure added (mirrored across both stacks)

| HC.AI.MAPI | HC.AI.FAPI | Purpose |
|---|---|---|
| `Agents/AgentV0.cs` | `agents/agent_v0.py` | accept request -> build query -> call tool -> format response |
| `Tools/HealthcareQueryTool.cs` | `tools/healthcare_query_tool.py` | wraps the (not-yet-built) `execute_healthcare_query` call |
| `Models/QueryRequest.cs`, `QueryFilter.cs`, `QueryOrderBy.cs`, `QueryResult.cs` | `models/query_dsl.py` | the locked query DSL shape: `table`, `select`, `filters`, `orderBy`, `limit` |
| `Utilities/PatientApiClient.cs` | `utils/http_client.py` | shared HTTP client to `AI.HealthCare.Patient.API` |
| `Controllers/` (existing) | `routers/ai_search.py` | will host the new `ai-search` endpoint |

Both sides started as pure typed skeletons (`throw new NotImplementedException()` /
`raise NotImplementedError`) before any LLM wiring, confirmed compiling clean.

## LLM layer built on the .NET side (`HC.AI.MAPI/Llm/`)

| File | Role |
|---|---|
| `OllamaOptions.cs` | binds `appsettings.json` `Ollama:BaseUrl` / `Ollama:Model` (defaults `http://localhost:11434`, `qwen2.5:7b`) |
| `IOllamaClient.cs` / `OllamaClient.cs` | thin wrapper over Ollama's `/api/chat`; `ChatAsync(systemPrompt, userPrompt, jsonResponse)` — `jsonResponse: true` sets Ollama's `format: "json"` so query-building responses come back as parseable JSON without a separate repair step |
| `OllamaChatModels.cs` | `OllamaMessage` / `OllamaChatRequest` / `OllamaChatResponse` — request/response DTOs for `/api/chat`, non-streaming (`stream: false`) |
| `HealthcareQueryPrompts.cs` | the two system prompts: `BuildQuerySystemPrompt()` (NL question -> query DSL JSON, allow-listed tables only, empty `table` if not a data lookup) and `BuildAnswerSystemPrompt()` (answer using ONLY the tool-result JSON, no fabrication) |

`AgentV0.cs` was then wired for real (not a stub anymore):

```
HandleRequestAsync(question)
  -> BuildQueryAsync: Ollama (json mode) -> QueryRequest
  -> HealthcareQueryTool.ExecuteQueryAsync(QueryRequest) -> QueryResult   [still NotImplementedException — Patient API endpoint doesn't exist yet]
  -> FormatResponseAsync: Ollama (plain text) using QueryResult JSON as the only source of truth
```

## Environment already in place (confirmed today)

- Ollama v0.32.0 running locally, `http://localhost:11434/api/version` responds
- `qwen2.5:7b` (4.7 GB) already pulled (`ollama list`)
- `appsettings.json` already points at both

## Next step (not done yet)

Smoke-test `qwen2.5:7b` directly via `curl http://localhost:11434/api/chat` with
`format: "json"` and the actual `HealthcareQueryPrompts.BuildQuerySystemPrompt()` text, to see
real model output before building the Patient API's `/api/ai-search/execute-query` endpoint that
`HealthcareQueryTool` needs to call.

## References

- Main plan / discussion log: [`2026-07-16_patient_ai_assistant_plan.md`](2026-07-16_patient_ai_assistant_plan.md)
- Design docs: [`healthcare_ai_assistant_mcp_ollama_design.md`](../../../architecture/design_patterns/healthcare_ai_assistant_mcp_ollama_design.md), [`healthcare_ai_assistant_semantic_kernel_design.md`](../../../architecture/design_patterns/healthcare_ai_assistant_semantic_kernel_design.md)
- Source: [`hc_ai_in/mapi/HC.AI.MAPI/`](../../../../hc_ai_in/mapi/HC.AI.MAPI/), [`hc_ai_in/fapi/HC.AI.FAPI/`](../../../../hc_ai_in/fapi/HC.AI.FAPI/)
