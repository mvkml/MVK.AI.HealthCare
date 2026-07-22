# Demo Deck — Part 5: Steps to Create/Extend a Custom Model from `qwen2.5:7b`

**Purpose of this file**: an actionable, checkable step list (not narrative) to actually build a
custom Ollama model extending this project's base model, `qwen2.5:7b`. Consolidates the concepts
from [`03_modelfile_guardrails.md`](03_modelfile_guardrails.md) (guardrail SYSTEM prompt) and
[`04_modelfile_customization_options.md`](04_modelfile_customization_options.md) (full Modelfile
syntax) into the actual sequence to run and verify.

| # | Step | Command / Action | Status |
|---|---|---|---|
| 1 | Confirm the base model is pulled | `ollama pull qwen2.5:7b`, then `ollama list` to confirm it's present | Not started |
| 2 | Create a `Modelfile` (any local folder, e.g. this repo's `hc_agile/hc_llm/ollama/`) | `FROM qwen2.5:7b` as the first line | Not started |
| 3 | Add the `SYSTEM` guardrail prompt | Grounding rule from the locked design doc: *"Only answer using tool call results. If a tool returns nothing, say so. Never state a fact not present in a tool result."* — see Part 3 for the full enterprise-example phrasing to adapt | Not started |
| 4 | Add `PARAMETER` tuning | Recommend low `temperature` (e.g. `0.1`-`0.2`) for reliable structured JSON output — matches this project's query-DSL reliability requirement (Part 1 §8); add `num_ctx`, `repeat_penalty` as needed per Part 4's table | Not started |
| 5 | Build the custom model | `ollama create healthcare-doctor -f Modelfile` — expect `success` output | Not started |
| 6 | Verify it was created correctly | `ollama list` (confirm `healthcare-doctor` appears) and `ollama show healthcare-doctor --modelfile` (confirm the baked-in `SYSTEM`/`PARAMETER` values match what you wrote) | Not started |
| 7 | Smoke-test the custom model directly | `ollama run healthcare-doctor` or `curl http://localhost:11434/api/chat -d "{\"model\":\"healthcare-doctor\",...}"` — confirm it answers in-character and refuses out-of-scope questions | Not started |
| 8 | Point the application at the new model | Update `Ollama:Model` in `appsettings.json` (or `Ollama__Model` env var) from `qwen2.5:7b` to `healthcare-doctor` — see [`02_ollama_configuration_steps.md`](02_ollama_configuration_steps.md) Steps 6-8 for exactly where this lives in code | Not started |
| 9 | Re-run the end-to-end demo checklist | Same checklist as Part 2 §10 / `001demo/README.md`, but confirming responses now come from `healthcare-doctor` | Not started |
| 10 | Keep the base model available as fallback | Do not delete `qwen2.5:7b` (`ollama rm` not run) — keep it pullable/runnable directly in case the custom model needs to be bypassed for debugging | Not started |

## Notes

- This is additive, not destructive — `qwen2.5:7b` remains untouched; `healthcare-doctor` is a
  new, separate named model built on top of it (per Part 3's Modelfile mechanics).
- Reminder from Part 3's limitation section: the `SYSTEM` prompt here is a guardrail, not a
  security boundary — this project's actual safety enforcement is structural (query DSL +
  server-side allow-list), and the custom model is defense-in-depth on top of that, not a
  replacement.
- Not yet executed/adopted — this file is the plan to check off, not a record of a completed
  change. Update the Status column as each step is actually run.

## Status

Drafted, not yet locked. All steps unexecuted — pending your go-ahead to run them.
