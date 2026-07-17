# Product Backlog

List of all features, enhancements, and fixes for the mvkhc project.
Prioritized by the Product Owner.

| ID    | Title                                                          | Priority | Status      |
|-------|-----------------------------------------------------------------|----------|-------------|
| PB001 | Explore Synthea patient dataset via PySpark (hc_bigdata)         | High     | In Progress |
| PB002 | Remove leftover HR-domain artifacts from hc_bigdata (tech debt)  | High     | To Do       |
| PB003 | Build AI.HealthCare.Patient.API (.NET, patient records)          | High     | To Do       |
| PB004 | Build AI.HC.Api core healthcare API (.NET)                       | High     | To Do       |
| PB005 | Setup clinical document intelligence functions (df_id_extractor, df_notes, fa_upload_doc) | Medium | To Do |
| PB006 | Build hc_ui health web portal (aihcweb, Angular)                 | Medium   | To Do       |
| PB007 | Setup Playwright API test suite (hc_qa)                          | Medium   | To Do       |
| PB008 | Design SQL schema / data source layer (hc_data_source)           | Medium   | To Do       |
| PB009 | Setup DevOps pipelines (hc_ai_ops)                                | Low      | To Do       |
| PB010 | Build health demo app & script (hc_demo)                          | Low      | To Do       |
| PB011 | Healthcare AI Assistant — natural-language query for Doctor persona (hc_ai_in/mapi, US007) | High | In Progress |
| PB012 | Chat UI for Doctor persona (Angular, US008) — consumes US007's Chat REST API | High | In Progress |
| PB013 | Stored procedure(s) for Doctor Chat prompt data access (US007, `hc_data_source/hc_sql`) | High | Deployed to LocalDB + validated |
| PB014 | ADR: reconcile "dynamic query with LLM-defined joins" against the locked "structured DSL, not raw SQL" decision | High | To Do |
| PB015 | Extend `HC.AI.MAPI.Models.QueryRequest` (C#) with a `Join` field so the Tool Layer can pass a join shape down to `usp_ExecuteHealthcareQuery` | Medium | To Do |
| PB016 | Resolve `@ProviderId` scoping policy for "recent patients" (hospital-wide vs. per-doctor) — needs Product Owner / Architect sign-off | Medium | To Do |
| PB017 | Harden `usp_ExecuteHealthcareQuery` for production: dedicated read-only SQL login/role, review `QueryAuditLog` retention, index review for whitelisted join columns | Medium | To Do |
| PB018 | Log rejected/guardrail-blocked query attempts to `QueryAuditLog` (currently only successful executions are audited — found during PB013 validation) | Medium | To Do |

## Notes
- PB001 is the only item with verified work: PySpark environment is confirmed working and the `allergies` Synthea table has been read and profiled. 18 more Synthea tables (patients, encounters, conditions, medications, procedures, claims, etc.) are present in `hc_bigdata/data/patient_details/synthea/` and not yet explored.
- PB002 exists because `hc_bigdata` currently carries copy-pasted HR-project residue: `data/employees.csv` (HR dummy data), setup docs referencing the `hr_bigdata` app name, and `.claude/settings.json` permissions pointing at `ai_hr\hr_bigdata` paths. Clean up before building further on top of it.
- PB003–PB007 all have folder/project scaffolding in place (layered .NET solutions, Angular app shell, Playwright fixtures/tests dirs) but **zero implementation files** — these are "To Do" from scratch, not partially done.
- PB008–PB010 have empty directory skeletons only, with no defined scope yet.
- PB013 raised by Dev SQL Agent. User specified: LLM Tool Layer sends a table/query shape + a max-records parameter (default 10), and the query needs to support joins across patient-related tables, decided dynamically at runtime since the LLM's exact query shape isn't known in advance. First version implemented in `hc_data_source/hc_sql/` (`usp_ExecuteHealthcareQuery` + whitelist/audit tables) as a **structured, whitelist-validated** dynamic query, not raw dynamic SQL text — see PB014 for why raw SQL was not used.
- PB014 exists because the user's request as stated ("dynamic query", "whatever the LLM builds", raw query text + provider type) is the literal ❌ option in `healthcare_ai_assistant_mcp_ollama_design.md` §4's risk table (LLM-generates-SQL -> SQL-injection-by-LLM). Implemented PB013 using the ✅ option instead (whitelisted table/column/join metadata + parameterized `sp_executesql`), but this needs an explicit ADR so the decision is recorded, not just implied by the code.
- PB016's "provider type" language from the original ask most likely maps to the already-open scoping question in the design doc §6/§10 (is "recent patients" hospital-wide or scoped to the asking doctor's own `ProviderId`?) — `usp_ExecuteHealthcareQuery` accepts an optional `@ProviderId` to support either behavior, but which one is the *default* is a product decision, not a SQL one.
