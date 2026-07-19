# Demo 2 discussion — Classification-driven routing (2026-07-19)

**Status:** Discussion only. Nothing implemented. User is providing more detail before any code
is written — do not build any part of this until the open items below are resolved.

## Context
Demo 1 (`provide-prompt`, Doctor persona, single hardcoded prompt) is complete and locked. Demo 2
introduces a classification step before the chat response, so the system can route a request to
the right handling instead of always using the same Doctor prompt.

## Understanding so far (as told to Claude, unconfirmed)

| # | Element | Understanding |
|---|---|---|
| 1 | Classification model | Same underlying model already wired (`qwen2.5:7b`), used for classifying rather than chatting |
| 2 | Two model files | (a) A new **Classification model** ("HC Classification" — Healthcare Classification) (b) The existing Demo 1 chat-response model (`PromptModel`/`PromptRequest`/`PromptResponse`) |
| 3 | What gets classified | Whether a request is "Doctor information" vs. "Client information" — exact taxonomy not finalized |
| 4 | Initial scope | Starting with one persona's perspective first — stated as "Patient perspective," flagged as possibly meaning "Doctor perspective" given Doctor has been the primary persona throughout; needs confirmation |
| 5 | Flow | `Request -> REST API -> Classification agent runs -> result passed to a Factory -> Factory selects/produces the right function for that classification -> function executes, returns the associated prompt information` |
| 6 | Purpose of the Factory | Intent/persona-based routing — dispatch to the correct downstream prompt/handling based on the classification result, rather than always using the single Doctor prompt |

## Open items (must be resolved before implementation starts)
- Exact classification taxonomy (categories, and how many)
- Confirm "Patient perspective" vs. "Doctor perspective" as the initial scope
- Where does the Classification model/agent live architecturally (new layer? inside `AL`
  alongside `DoctorSemanticProcess`? a new persona-agnostic classifier ahead of `AL`?)
- Relationship between the new Factory (for classification-based routing) and the existing
  `IKernalFactory` (for LLM Kernel creation) — same factory, or a second one with a different
  responsibility?
- How "HC Classification" model file relates to the existing `PromptModel`/`PromptItem` shape —
  new sibling model, or an extension of `PromptItem`?

## Next step
Wait for the user to provide the remaining detail before any design or code work begins.
