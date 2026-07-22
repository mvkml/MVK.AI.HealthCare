# mvkhc Wiki

Local, version-controlled wiki — an archive of architecture flow and QA process documentation,
organized **module-wise** (one folder per module/feature) and **version-wise** (each update adds
a new dated version file instead of overwriting the last one, so history is preserved and can be
revisited later).

Mirrors the structure already used for the Azure DevOps Wiki's LLM section
(`hc_agile/hc_llm/ollama/`, synced to ADO as `Large Language Models (LLM)` → `Ollama` → numbered
pages) — this folder is the local source of truth, and a candidate for the same kind of ADO Wiki
sync later, once there's a defined root page for it.

## Sections
- [`qa/`](qa/) — QA folder structure snapshots, test coverage per module, Epic/Feature/Story
  hierarchy snapshots

## Convention
Each module gets its own folder. Each update to that module's documentation is a new file named
`v<N>_<YYYYMMDD>.md` — never edit an existing version file in place; add the next version instead.
Each module folder's own `README.md` (if present) points at the current/latest version.
