# TASK019 - Complete persona-based model selection (PB034)

**Status:** Done — 2026-07-21. See
[worklog](../../worklogs/dev_semantic_kernel/20260721_180000_persona_model_selection_wired.md).
**Assigned:** Dev Semantic Kernel Agent (with Dev Angular Agent flagging the bug that started this)

## Why this exists
Dev Angular Agent found that `DoctorPromptMapper` hardcoded the **Patient** persona constant on
the **Doctor** endpoint's mapper, while reviewing a new `Persona` field that had appeared on
`PromptRequest`. Investigating further (and picking up concurrent Semantic Kernel work already in
flight on the same file) found the mapper had since been corrected to compute a `ModelKey` from
the real persona — but `LLMModelBL`, the class that actually resolves which LLM options to use,
still ignored it and always hardcoded the Doctor executor section.

## Scope
- `LLMModelBL.GetModelDetails`: resolve `model.PromptItem.ModelKey` instead of the hardcoded
  `APIConstants.DoctorExecutorPersonaName` constant
- Update `ILLMModelBL`/`LLMModelBL` doc comments to match (they still described the old
  always-Doctor behavior)
- Angular: add `persona` to `doctor-chat.service.ts`'s `PromptRequest` interface, send `'Doctor'`
  explicitly from the Chat UI

## Explicitly out of scope
- Database-backed persona/model config — PB019/PB032/EPIC001, separate and larger
- Building a real Patient chat endpoint — Patient chat UI is still mock-only (PB024); this task
  only makes the *existing* Doctor endpoint correctly branch when a Patient-persona request
  reaches it, it doesn't create a new endpoint for that persona

## Backlog reference
`BACKLOG.md` PB034.
