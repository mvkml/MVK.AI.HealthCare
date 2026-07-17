# Doctor Agent — first end-to-end slice (2026-07-17)

Implements TASK006/US007's first acceptance criterion: `UCDoctorController` responds end-to-end,
proving the full request path for the Doctor persona.

## What was built

Persona controllers (8, structural only, from the earlier persona discussion):
`UCDoctorController`, `UCPatientController`, `UCProviderController`, `UCHospitalController`,
`UCInsuranceProviderController`, `UCUserController`, `UCClientController`, `UCAnonymousController`
— only `UCDoctorController` has real logic so far.

Full chain wired for the Doctor persona, one step at a time:

```
GET /api/UCDoctor?message=hi
  -> UCDoctorController          (HC.AI.MAPI, Controllers/)
  -> IDoctorService / DoctorService   (HC.AI.MAPI, Services/ -- new "Service Layer" folder)
  -> IDoctorAgent / DoctorAgent       (HC.AI.MAPI.AL)
```

`IDoctorAgent` has two methods, reflecting a deliberate split confirmed during the session:
- `BasicHandleRequestAsync(string message)` -- hardcoded response, no dependencies at all. This
  is the "hi" case: no Business Layer, no LLM, nothing downstream. Proves the plumbing only.
- `HandleRequestAsync(string message)` -- the "proper" agent: injects `IOllamaClient`
  (`HC.AI.MAPI.Llm`) and makes a real call to Ollama (`qwen2.5:7b`) with a simple system prompt,
  returning the model's actual response.

`UCDoctorController` currently calls `HandleRequestAsync` (the real LLM path) via `DoctorService`
-- meaning hitting this endpoint right now makes a live call to the local Ollama instance, not
the hardcoded `BasicHandleRequestAsync` response. Flagged as an open item: may want to switch to
`BasicHandleRequestAsync` for a dependency-free smoke test first.

## Naming decisions confirmed along the way

- `DoctorService` (not `ProviderService`) for the Service Layer class -- no naming clash with the
  `Provider` entity, and `Doctor` already carries the "primary client, higher accuracy" framing
  locked earlier in `2026-07-17_service_layer_architecture_discussion.md`.
- Kept `Doctor` and `Provider` as separate concepts for now -- `Provider` (`UCProviderController`)
  stays an unimplemented placeholder until a real feature needs it; collapsing them would lose
  the Doctor-specific accuracy commitment.
- `DoctorAgent` created alongside the existing `AgentV0` (not replacing it) -- `AgentV0` is left
  untouched as the earlier generic/consolidated agent.
- No Semantic Kernel used for `DoctorAgent` -- stayed consistent with `AgentV0`'s existing
  hand-rolled `IOllamaClient` approach (raw HTTP to Ollama's `/api/chat`), rather than introducing
  a second LLM-calling mechanism in the same `HC.AI.MAPI.AL` project.
- `HelloWorldBL` was explicitly rejected as a dependency for `DoctorAgent` -- it was only ever the
  proof-of-concept for the plain Controller -> BL path, not meant to carry real Doctor logic.
- Confirmed no Business Layer call is needed at all for the "hi" slice specifically -- `AL -> BL`
  stays unexercised until a real query (patient lookup) is built.

## Backlog formalization (also done today)

- **US007** (`hc_agile/product_owner/user_stories/US007_healthcare_ai_assistant.md`) -- Doctor
  persona natural-language query, first AC is this exact "hi" slice.
- **PB011** added to `hc_agile/product_owner/backlog/BACKLOG.md`.
- **TASK006** (`hc_agile/scrum/tasks/TASK006_US007_doctor_controller_hello_slice.md`) -- the
  concrete task this session's work completes.
- Sprint: still unscheduled (only `sprint_01` exists in this repo).

## NuGet packages used so far (entire HC.AI.MAPI solution)

| Package | Version | Project | Why |
|---|---|---|---|
| `Swashbuckle.AspNetCore` | 6.4.0 | `HC.AI.MAPI` | Default `dotnet new webapi` template package -- Swagger/OpenAPI UI |
| `Microsoft.Extensions.Options` | 10.0.10 | `HC.AI.MAPI.Llm` | Needed once `Llm` was split out of the API project into its own class library -- `OllamaOptions` binding (`IOptions<OllamaOptions>`) isn't available for free outside the ASP.NET Core Web SDK |

No Semantic Kernel, no LangChain/LangGraph, no HTTP client libraries beyond the built-in
`System.Net.Http.HttpClient` -- everything else in the solution (11 class libraries) has zero
external dependencies.

## References

- [`2026-07-17_service_layer_architecture_discussion.md`](2026-07-17_service_layer_architecture_discussion.md) -- persona controller naming discussion, Guardrail/Prompt Layer decisions
- [`2026-07-17_mapi_folder_scaffold_and_llm_layer.md`](2026-07-17_mapi_folder_scaffold_and_llm_layer.md) -- original Llm layer build
- Source: [`hc_ai_in/mapi/HC.AI.MAPI/`](../../../../hc_ai_in/mapi/HC.AI.MAPI/)
