# Implementation Guide — Design Patterns & SOLID Principles

Owned by the Architect Agent. Applies to `HC.AI.MAPI` first (where it was written, triggered by
the Doctor endpoint vertical), but the same rules apply project-wide as other verticals
(`AI.HC.Api`, `AI.HealthCare.Patient.API`, the future `hc_ai_in/fapi` FastAPI+LangGraph build)
reach the same maturity. Revisit and extend this doc as new layers get implemented — patterns
get added here once a real need shows up in code, not speculatively.

## SOLID — how each principle applies here

| Principle | What it means in this codebase | Current status |
|---|---|---|
| **S**ingle Responsibility | Each class does one job: Controller = HTTP concerns only, Service = orchestration, Agent = persona logic, PromptProvider = prompt construction | ✅ Followed so far — keep every new class to one reason to change |
| **O**pen/Closed | Adding a new LLM provider, a new persona, or a new query type should mean *adding* a class, not editing a `switch` | 🟡 Gap found: `KernalFactory.CreateKernel` switches on `Provider` with only a `default` case — fix via Strategy pattern (below) before adding provider #2 |
| **L**iskov Substitution | Any `IDoctorAgent`/`IDoctorPromptProvider`/`ISemanticProcessService` implementation must be swappable without breaking callers | ✅ No violations yet — keep interfaces behavior-contracted, not just shape-contracted, as more implementations appear |
| **I**nterface Segregation | Interfaces stay small and role-specific (`IDoctorService`, `IKernalFactory`, `IPromptValidationUtility`) rather than one fat `IHealthcareAI` interface | ✅ Followed |
| **D**ependency Inversion | Every class depends on an interface via constructor injection, registered in `Program.cs` | ✅ Followed consistently — continue this for every new class, no `new SomeService()` inside business logic |

## Design patterns already in use

| Pattern | Where | Notes |
|---|---|---|
| **Dependency Injection / IoC** | Everywhere (`Program.cs` DI container) | Foundation for testability — keep it non-negotiable |
| **Factory Method** | `KernalFactory.CreateKernel(LLMOptions)` | Shape is right, implementation is incomplete (see OCP gap above) |
| **Options Pattern** | `IOptions<OllamaOptions>` | Standard .NET config-binding pattern, correctly used |
| **Context Object / Envelope** (see [ADR001](../decisions/ADR001_prompt_model_context_object.md)) | `PromptModel` travels Controller→Service→BL→SemanticProcess as a single in/out parameter — never a bare `PromptItem`/`LLMOptions` sub-piece | Trades signature type-safety (missing required field is a runtime bug, not a compile error) for schema flexibility — new fields never ripple through every layer's signature. Mitigated by documenting field preconditions in XML doc comments |
| **Mapper** | `HC.AI.MAPI.SemanticProcess.Mapping.PromptRequestMapper` (static — Request→execution settings, result→`PromptResponse`); `HC.AI.MAPI.Services.Mapping.IDoctorPromptMapper`/`DoctorPromptMapper` (DI-injected — Request→`PromptItem`, Options→`LLMOptions`, →`PromptModel`) | No AutoMapper. `DoctorPromptMapper` is injected via constructor like every other class in this codebase (not static) — keeps it mockable/testable and consistent with the DI convention. Keeps `DoctorService` down to pure orchestration — no mapping logic lives in the Service itself |

## Design patterns to incorporate — catalog with concrete application

These aren't adopted yet. Each row is a candidate, mapped to a real gap found while building the
Doctor endpoint vertical. Pull one in when the layer it applies to is actually being built —
don't pre-build for patterns with no current caller.

| Pattern | Apply it to | Why |
|---|---|---|
| **Strategy** | `KernalFactory` → one `ILlmProviderStrategy` per provider (Ollama today, others later), selected by `Provider` string via DI-registered dictionary instead of a `switch` | Fixes the OCP gap directly — adding a provider becomes "add a class + register it," not "edit the factory" |
| **Adapter** | `OllamaClient`, `PatientApiClient` | Both already *are* adapters wrapping an external system (Ollama runtime, `AI.HealthCare.Patient.API`) behind our own interface — worth naming explicitly so new integrations (e.g. a future EHR system) follow the same shape |
| **Chain of Responsibility** | Validation pipeline for `PromptRequest` and `QueryRequest` | Today `PromptValidationUtility.Validate` is a single method with one check. As more rules appear (token limits, banned content, guardrail table/column whitelist), chain small validators instead of growing one method |
| **Decorator** | Guardrail layer wrapping `ISemanticProcessService`/the future `DoctorSemanticProcess` | Read-only enforcement, record limits, and allow-listed table/column checks are cross-cutting — a decorator around the core execution keeps guardrail logic out of the business logic and independently testable |
| **Specification** | `QueryFilter` / `QueryOrderBy` / `QueryRequest` (already shaped like the start of this pattern) | Formalize into composable specifications so `HealthcareQueryTool` builds dynamic, whitelist-validated queries without string-concatenation risk (ties directly into backlog item PB014's SQL-injection-by-LLM concern) |
| **Circuit Breaker / Retry** (via Polly) | `OllamaClient` calls | Local LLM runtime (Ollama) can be slow/unavailable — wrap calls with retry + circuit breaker instead of letting failures propagate raw to the controller |
| **Builder** | Constructing `ChatHistory`/`KernelArguments` for calls with many optional parameters | `PromptRequest` has 9+ optional generation parameters — a builder keeps construction readable as more get added |
| **Null Object** | `PromptResponse` error/empty states | Return a well-formed `PromptResponse` with `IsSuccess = false` and `ErrorMessage` set, instead of nulls, so callers never null-check |
| **Repository** | `HC.AI.MAPI.Repository` (currently empty) | Only needed once `HC.AI.MAPI` owns its own persistence directly (e.g. `QueryAuditLog`, cached `PromptItem` history) rather than reaching everything through `PatientApiClient` |
| **Unit of Work** | Any future flow that writes to more than one table/table-set atomically (e.g. query execution + `QueryAuditLog` entry together) | Not needed yet — `usp_ExecuteHealthcareQuery` currently handles this at the SQL layer |
| **Observer / Event-driven** | Audit logging (`QueryAuditLog`, including PB018's rejected-query logging) | Emit an event on every guardrail decision (allowed/blocked) rather than hardcoding the audit-log call inside the query execution path — keeps PB018 additive instead of another edit to the same method |

## Validation layer — target shape

- One validator per request model (`PromptRequest`, `QueryRequest`), each implementing a small
  `IValidator<T>`-style interface (Chain of Responsibility for composability, not a framework
  dependency).
- Every public entry point (Controller action) validates before calling the Service layer — this
  closes the gap where the 3 `GET` endpoints currently skip validation entirely.
- Range-check every numeric field on `PromptRequest` (`Temperature` 0–2, `TopP` 0–1, `MaxTokens` >
  0, etc.) — none of this exists today.

## Mapper layer — target shape

- A small, explicit mapper class per direction (`PromptRequestToKernelArgumentsMapper`,
  `FunctionResultToPromptResponseMapper`) — static extension methods are enough; no AutoMapper
  dependency needed at this project's size.
- Mappers live next to the layer that owns the *target* type — e.g. the `PromptRequest` →
  Semantic Kernel mapper belongs in `HC.AI.MAPI.SemanticProcess` (or wherever `TASK008`'s open
  question #2 resolves the execution layer to), not in `Models`.

## How this guide gets used

- Before implementing a new layer or fixing a gap listed above, check this table first — if a
  pattern is already named here, follow the shape described rather than inventing a new one.
- When a pattern gets adopted, move its row from "to incorporate" to "already in use" with a
  one-line note on where, in the same PR/commit that implements it.
- Any deviation from SOLID (e.g. a class that must know about 3 unrelated things) should be
  called out explicitly in code review, not silently merged.
