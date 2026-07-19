# ADR001: PromptModel as a Context Object Across All Layers

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-07-18 | Initial decision |
| 1.1 | 2026-07-18 | `PromptItem` established as the self-sufficient execution unit within the envelope; `PromptRequest` restricted to intake-only (see "Item vs Model vs Request" below) |

## Status

Accepted.

## Context

While wiring the Doctor endpoint (US007/TASK008), layers were passing individual pieces around —
e.g. `ILLMModelBL.GetModelDetails(PromptItem promptItem)` took a `PromptItem` and returned a
bare `LLMOptions`. Every time a new field is needed on `PromptItem`, `PromptRequest`,
`PromptResponse`, or `LLMOptions`, every function signature that touches that data has to be
found and updated across every layer (Controller → Service → BL → SemanticProcess), and every
test has to be adjusted for the new parameter shape.

## Decision

`PromptModel` (Request + Item + Response + LLMOptions, see `HC.AI.MAPI.Models.Prompt.PromptModel`)
is the single envelope that travels through every layer of the Doctor chat flow. The rule:

- Every layer function takes a `PromptModel` in and returns a `PromptModel` out — never a bare
  `PromptItem`, `LLMOptions`, or other sub-piece as a parameter or return type.
- A layer reads whatever sub-object it needs off the model (e.g. `model.PromptItem`), does its
  work, assigns the result back onto the model (e.g. `model.LLMOptions = ...`), and returns the
  same model.
- The only exception is the very first construction point (Controller/Service boundary), where a
  `PromptModel` doesn't exist yet and has to be built from the raw `PromptRequest`.

This is the **Context Object / Envelope pattern**. It trades some type-safety for schema
flexibility — adding a field later never requires touching function signatures across layers.

## Alternatives Considered

- **Individual typed parameters per layer** (status quo before this ADR) — most explicit/
  self-documenting, but every new field change ripples through every signature in the call chain.
- **Split DTOs per layer with mappers between each** — avoids the "God Object" concern below, but
  adds a mapper class per layer boundary, which is more ceremony than this project needs at its
  current size.

## Consequences

**Gain:** adding a field to `PromptItem`/`LLMOptions`/etc. touches only the class itself and
whichever layer actually uses the new field — no signature changes elsewhere. Tests build one
`PromptModel` and assert on whatever the layer under test is responsible for.

**Cost — accepted tradeoff:** a function signature like `ProcessAsync(PromptModel model)` no
longer tells you which fields it actually reads/writes just by looking at it, the way
`ProcessAsync(LLMOptions options, string prompt)` would. More importantly, a missing required
value (e.g. calling a layer before something upstream populated `model.LLMOptions`) is no longer
a **compile error** — it's a silent runtime bug (empty/default values flow through instead of a
build failure).

**Mitigation:** every method that requires a field to already be populated documents that
precondition in an XML doc comment (e.g. "requires `model.PromptItem` to be set"). This doesn't
give compiler enforcement back, but keeps the contract visible to the next person reading the
code.

## Applies To

`HC.AI.MAPI.AL.IDoctorSemanticProcess` (already followed this shape from the start),
`HC.AI.MAPI.BL.LLMModel.ILLMModelBL`, `HC.AI.MAPI.Services.Mapping.IDoctorPromptMapper` — and any
new layer added to the Doctor chat flow going forward.

## Amendment (v1.1): Item vs. Model vs. Request

The envelope holds three things with three different lifetimes and roles — they are not
interchangeable, and conflating them was the source of two issues caught in review:

- **`PromptRequest`** — intake only. Exists to receive and validate what the caller sent.
  `HC.AI.MAPI.Services.Mapping.DoctorPromptMapper.ToPromptItem` is the **only** place that reads
  it; every field on it (Message, generation parameters) gets copied onto `PromptItem` there.
  Nothing downstream of the Service layer — `ILLMModelBL`, `IDoctorSemanticProcess`,
  `ISemanticProcessService` — ever touches `model.PromptRequest` again.
- **`PromptItem`** — the actual, self-sufficient unit of execution. Carries everything a layer
  needs to do its work: `Message`, the generation parameters, `Persona`, and (once
  `ILLMModelBL.GetModelDetails` runs) `LLMOptions`/`LLMProvider`. This is the object every
  business-logic layer reads from and writes updates onto.
- **`PromptModel`** — the pipeline envelope (per the base decision above), bundling
  Request+Item+Response+LLMOptions for this one request's sequential flow through Controller →
  Service → BL → SemanticProcess.

**Why this split matters — parallel/fan-out execution:** a future orchestration scenario may need
to run multiple executions concurrently (e.g. fanning out to several personas/models at once).
`PromptModel` is shaped for one request's linear pipeline, not for splitting into N concurrent
branches. `PromptItem`, by design, has no dependency on the rest of the model — it can be pulled
out and handed to a parallel worker on its own. Keeping business-logic layers reading only from
`PromptItem` (never `PromptRequest`, and not tightly bound to `PromptModel`) means fan-out/fan-in
can be introduced later with zero impact on this design — every layer already operates on items.

**Consequence:** `PromptItem` now duplicates most of `PromptRequest`'s shape (Message + generation
parameters) plus carries its own `LLMOptions`. This is intentional duplication, not an oversight —
it's what makes the item portable/self-sufficient. `ISemanticProcessService.ExecutePromptAsync`
and `HC.AI.MAPI.SemanticProcess.Mapping.PromptItemMapper` (renamed from `PromptRequestMapper`)
were updated to take `PromptItem` accordingly.
