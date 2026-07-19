# TASK008 - Wire DoctorService -> DoctorSemanticProcess -> DoctorPromptProvider -> Semantic layer

**US:** US007
**Status:** Done — implemented, builds clean, verified live 2026-07-18. See ADR001 for the
Context Object pattern this flow follows, and
`hc_agile/worklogs/dev_semantic_kernel/20260718_180408_module1_doctor_prompt_locked.md` for the
locked end-to-end module writeup.

## Description
`DoctorService.GetChatResponseByPrompt` and `DoctorService.ProvidePromptAsync` both currently
`throw new NotImplementedException()`. The intended flow, discussed and confirmed twice in
conversation but not yet built:

`DoctorController` -> `DoctorService` (assigns `LLMOptions`) -> `DoctorSemanticProcess` (new,
not yet created — gets the prompt from `DoctorPromptProvider`, then calls the generic Semantic
layer with `llmOptions` + prompt) -> generic Semantic layer (creates a `Kernel` via a factory,
invokes the model, returns the raw response) -> back up through `DoctorSemanticProcess` (cleans
up/reshapes the raw response) -> `DoctorService` -> `DoctorController`.

Currently orphaned (exist, but nothing in the live code path calls them):
- `HC.AI.MAPI.Prompt.Doctor.DoctorPromptProvider` (Semantic Kernel `IChatCompletionService` +
  `ChatHistory`)
- `HC.AI.MAPI.Semantic` (`KernalFactory`/`IKernalFactory`, no execution method yet)
- `HC.AI.MAPI.SemanticProcess` (`ISemanticProcessService`/`SemanticProcessService`,
  `CreateFunctionFromPrompt` + `InvokeAsync`, no interface-based factory)

## Open questions — all resolved (implemented 2026-07-18)

| # | Question | Resolution |
|---|---|---|
| 1 | Where does `DoctorSemanticProcess` live? | `HC.AI.MAPI.AL`, alongside `DoctorAgent` — as recommended |
| 2 | Which generic Semantic layer does it call? | Both, composed: `SemanticProcessService` (`.SemanticProcess`) owns `CreateFunctionFromPrompt`/`InvokeAsync`, but gets its `Kernel` from `IKernalFactory` (`.Semantic`) — reconciled rather than one retired |
| 3 | Where does `LLMOptions` get constructed? | New `HC.AI.MAPI.BL.LLMModel.LLMModelBL`, reading `IOptions<OllamaOptions>` — isolates the provider/model decision as its own business-layer responsibility |
| 4 | What does "clean up the response" map to? | `PromptItemMapper.ToPromptResponse`/`ToErrorPromptResponse` — maps `Content`/`ModelUsed`/`LatencyMs`/`IsSuccess`; `FinishReason`/token counts documented as unsupported by the Ollama connector, not silently dropped |

See ADR001 (`hc_agile/architecture/decisions/ADR001_prompt_model_context_object.md`) for the
`PromptModel`/`PromptItem`/`PromptRequest` envelope pattern this implementation follows.
