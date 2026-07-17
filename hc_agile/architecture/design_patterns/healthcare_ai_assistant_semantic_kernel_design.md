# Healthcare AI Assistant — ASP.NET Core + Semantic Kernel Design (Option B)

**Status**: Alternative/candidate design — presented alongside the previously locked "fully
custom, no framework" approach. See §6 for the open decision this document raises.

## 1. What this is

Same overall goal as the main design (see
[`healthcare_ai_assistant_mcp_ollama_design.md`](healthcare_ai_assistant_mcp_ollama_design.md)):
natural-language patient/hospital lookup, grounded in SQL Server, using Ollama (`qwen2.5:7b`) as
the LLM. This document describes building the orchestration layer with **Semantic Kernel** (SK)
— Microsoft's open-source .NET orchestration library — instead of a fully hand-rolled loop.

**Important distinction, since this came up directly in discussion**: Semantic Kernel is
**not** the same thing as Azure AI Foundry Agent Service. SK is an open-source library you host
and run yourself, inside your own ASP.NET Core process, against any model backend — including
a fully local, on-prem Ollama instance. It has no Azure dependency and no cloud coupling. Using
SK does not pull Foundry into the picture.

## 2. Why Semantic Kernel over fully custom

| Concern | Fully custom (Option A, previously recommended) | Semantic Kernel (Option B) |
|---|---|---|
| Tool/function registration | Hand-write JSON schema + dispatch logic per tool | `[KernelFunction]` attribute on a C# method — SK generates the schema and handles dispatch automatically |
| Tool-calling loop (ask model -> model requests tool -> call it -> feed result back -> ask again) | Hand-written while-loop, hand-parsed tool-call JSON | Built into SK's `IChatCompletionService` / `FunctionChoiceBehavior.Auto()` — SK runs the loop for you |
| Ollama integration | Raw `HttpClient` to Ollama's `/api/chat` endpoint | SK has an Ollama connector (`Microsoft.SemanticKernel.Connectors.Ollama`, currently marked experimental) that points at the same local Ollama instance |
| Multi-agent orchestration (the 9-agent pipeline) | Each agent is a hand-written C# class, wired together manually | SK's plugin model + planners give a more structured way to compose steps, though the 9-agent pipeline can still be modeled as a manual chain of Kernel calls either way |
| Dependency risk | Zero external framework dependency | One additional NuGet dependency; SK is actively developed by Microsoft but the Ollama connector specifically is still experimental/pre-1.0 |
| Control / transparency | Full visibility into every step — easiest to debug and explain in a demo | Some mechanics (the tool-calling loop internals) happen inside the library — less line-by-line visibility unless you dig into SK's source |

## 3. How the 18-entity REST API maps to Semantic Kernel

Each MCP-style tool becomes a **Kernel Plugin** — a plain C# class with `[KernelFunction]`-
attributed methods. For Phase 1's single `execute_healthcare_query` tool:

```csharp
public class HealthcareQueryPlugin
{
    private readonly HttpClient _httpClient;

    public HealthcareQueryPlugin(HttpClient httpClient) => _httpClient = httpClient;

    [KernelFunction("execute_healthcare_query")]
    [Description("Runs a validated, read-only query against the healthcare database. " +
                 "Input is a structured query object: table, optional filters, optional " +
                 "orderBy, optional limit.")]
    public async Task<string> ExecuteQueryAsync(string queryJson)
    {
        var response = await _httpClient.PostAsync(
            "/api/ai-search/execute-query",
            new StringContent(queryJson, Encoding.UTF8, "application/json"));
        return await response.Content.ReadAsStringAsync();
    }
}
```

Registered once in `Program.cs`:

```csharp
var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId: "qwen2.5:7b", endpoint: new Uri("http://localhost:11434"))
    .Build();

kernel.Plugins.AddFromObject(new HealthcareQueryPlugin(httpClient));
```

Then a single call handles the entire "ask -> tool call -> feed result -> answer" loop:

```csharp
var settings = new OllamaPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};
var result = await kernel.InvokePromptAsync(userQuestion, new(settings));
```

This single `InvokePromptAsync` call replaces what would otherwise be a hand-written loop
(Agents 4 + 6 + 7 from the 9-agent pipeline — build query, call tool, format response) in the
fully-custom approach.

## 4. Where the rest of the 9-agent pipeline still applies

Semantic Kernel handles the mechanical tool-calling loop, but it does **not** replace the
higher-level agent design already locked:

- Agent 2 (Query-Check/Classifier) and Agent 5 (Validation/Guardrail — hallucination check,
  read-only enforcement, record-limit caps) still need to be explicitly built. SK does not
  provide hallucination detection or business-rule guardrails out of the box.
- Agent 3 (Information Agent fallback), Agent 8 (Consolidation), and Agent 9
  (Marketing/Branding wrapper) remain custom logic sitting before/after the SK-driven core loop.

So the realistic picture is: **SK replaces the innermost mechanical loop (build query -> call
tool -> get result), the surrounding pipeline (classify, validate, consolidate, brand) is still
hand-written regardless of which option is chosen.**

## 5. Package/setup requirements

- `Microsoft.SemanticKernel` (core, stable)
- `Microsoft.SemanticKernel.Connectors.Ollama` (experimental — API may change between SK
  versions; worth pinning the version and re-testing on any SK upgrade)
- No Azure resources, no Foundry project, no cloud dependency — everything points at the local
  Ollama instance (`http://localhost:11434`) and the existing ASP.NET Core API

## 6. Open decision — Option A vs. Option B

This document exists as a second candidate alongside the previously locked "fully custom, no
framework" approach (see the worklog entry titled "Agent implementation approach in ASP.NET
Core — Foundry coupling question"). Both are valid and neither is Foundry-coupled. The
trade-off is genuinely: **less boilerplate + built-in tool-calling loop (SK)** vs. **full
transparency + zero dependency risk (custom)**.

**Not yet decided which one Phase 1 will actually use** — flagging this explicitly rather than
silently overriding the earlier lock. Needs a decision before implementation starts.

## References

- Main technical design: [`healthcare_ai_assistant_mcp_ollama_design.md`](healthcare_ai_assistant_mcp_ollama_design.md)
- Full chronological discussion log: [`hc_agile/worklogs/learn/ai_mcp_ollama/2026-07-16_patient_ai_assistant_plan.md`](../../worklogs/learn/ai_mcp_ollama/2026-07-16_patient_ai_assistant_plan.md)
