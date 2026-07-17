# hc_data_source/hc_sql

Owned by: Dev SQL Agent.

## Doctor Chat prompt dynamic query support (PB013)

Supports US007 (Doctor Chat, `hc_ai_in/mapi/HC.AI.MAPI`): the LLM-driven Tool Layer needs to run
queries whose shape (columns, filters, one join, row limit) isn't known until runtime. Per the
locked design in
[`healthcare_ai_assistant_mcp_ollama_design.md`](../../hc_agile/architecture/design_patterns/healthcare_ai_assistant_mcp_ollama_design.md)
§4, the LLM never generates raw SQL — it generates a structured query shape, validated
server-side before touching the database.

- `tables/001_whitelist_and_audit_tables.sql` — `TableWhitelist`, `ColumnWhitelist`,
  `JoinWhitelist` (server-side allow-list; no identifier from the caller reaches dynamic SQL
  without passing through these) and `QueryAuditLog` (every execution, for compliance
  traceability).
- `seed/001_doctor_persona_whitelist_seed.sql` — Phase 1 Doctor-persona scope: `Patients`,
  `Encounters`, `Conditions`, `Providers`, `Organizations` (real table names in
  `AI_HealthCarePatient`, EF Core's pluralized convention — matches the design doc's domain map,
  Insurance/Billing excluded).
- `procedures/usp_ExecuteHealthcareQuery.sql` — the stored procedure. Takes root table, optional
  single join, select/filter/order shape (max 5 filters), and `@MaxRecords` (default 10, hard
  ceiling 50). Table/column/join identifiers are resolved only from the whitelist tables above
  (via `QUOTENAME`); filter values are always bound `sp_executesql` parameters, never
  concatenated.

Run order: `tables/` then `seed/` then `procedures/`.

**Open items** — see `PB014`–`PB016` in the product backlog: an ADR is needed to formally
reconcile "dynamic query with joins" against the locked "no raw SQL" decision; the C# query DSL
(`HC.AI.MAPI.Models.QueryRequest`) needs a `Join` field to carry this from the LLM Tool Layer down
to this procedure; and the `@ProviderId` scoping behavior (hospital-wide vs. per-doctor "recent
patients") needs Product Owner / Architect sign-off, not just a default.
