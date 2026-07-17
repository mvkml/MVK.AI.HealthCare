# Healthcare AI Assistant — MCP + Ollama Technical Design

**Status**: Phase 1 design locked, implementation starting. **Model pulled**: `qwen2.5:7b`.

## 1. Problem statement

Enable natural-language lookup over the existing `AI.HealthCare.Patient.API` (18 entities,
SQL Server) — e.g. a doctor asks "recent patients" or "details for John Smith" in plain
English, and gets an answer grounded entirely in the real database. The system must never
fabricate patient data: every fact in a response must trace back to a real query result.

## 2. Why not fixed per-question endpoints

A doctor (or later, other personas) can ask virtually anything — "patients on medication X",
"patients over 50 with condition Y", "recent visits by Dr. Z". Hand-writing a REST
endpoint/MCP-tool per possible question is combinatorial and doesn't scale. Instead, the system
generates the query dynamically from the question, using knowledge of the database schema.

## 3. Architecture

```
User enters a question (e.g. patient name, "recent patients")
  -> Ollama (qwen2.5:7b, tool-calling enabled)
  -> MCP client
  -> MCP server (wraps the existing REST API) — single tool: execute_healthcare_query
  -> AI.HealthCare.Patient.API (unchanged) — new generic query endpoint
  -> SQL Server
  -> results flow back up as JSON
  -> Ollama synthesizes an answer using ONLY the tool result (never its own knowledge)
```

The existing REST API is not modified in its per-entity CRUD/import behavior — this adds one
new endpoint alongside it.

## 4. Core design decision: structured query DSL, not raw SQL

Ollama does **not** generate SQL text. It generates a constrained JSON query object, which the
API validates against a schema allow-list and translates to parameterized EF Core LINQ.

| Approach | Risk |
|---|---|
| ❌ Ollama generates raw SQL, API executes directly | SQL-injection-by-LLM — even a "correct" query can be wrong in scope (e.g. missing a `PatientId` filter leaks all patients), or occasionally malformed/destructive |
| ✅ Ollama generates a structured JSON query object; API validates + translates to LINQ | Safe — API only ever executes LINQ it built itself; enforces read-only, allow-listed tables/columns only |

**Query DSL v1 (minimal, expand later):**

```json
{
  "table": "Encounter",
  "select": ["PatientId", "Start", "Description", "ReasonDescription"],
  "orderBy": { "field": "Start", "direction": "desc" },
  "limit": 10
}
```

```json
{
  "table": "Patient",
  "filters": [
    { "field": "First", "op": "eq", "value": "John" },
    { "field": "Last", "op": "eq", "value": "Smith" }
  ]
}
```

v1 supports: `table`, `select`, `filters` (equality only), `orderBy`, `limit`. Expand later with
more operators (`contains`, `gt`/`lt`, `between`) and cross-table joins as new question types
come up — one capability at a time, demo by demo.

**Safety enforcement (server-side, non-negotiable):**
- Table/column names validated against an allow-list — the same field inventory documented in
  the accompanying worklog (see §8) serves as both the schema context given to Ollama and the
  server-side validation list.
- Read-only only — no code path from this endpoint to INSERT/UPDATE/DELETE, by construction
  (the DSL has no concept of a write operation).
- Malformed or unrecognized query shapes are rejected before touching the database.

## 5. MCP tool surface (Phase 1)

One tool: `execute_healthcare_query` — input is the query DSL JSON above, output is the raw
JSON result set. Ollama's system prompt instructs it to: (a) only answer using this tool's
results, (b) say so explicitly if the tool returns nothing, (c) never state a fact not present
in a tool result.

## 6. Persona and scope — Phase 1

Doctor persona first (not patient or insurance yet). Two query shapes to support:

| Query type | Example | Shape |
|---|---|---|
| Single-patient lookup | "Patient details for John Smith" | Search by name -> one patient's record |
| Aggregate / list query | "How many patients are there", "recent visiting patients" | Scans across patients/encounters |

Open question, not yet resolved: is "recent visiting patients" hospital-wide or scoped to the
asking doctor's own patients (`ProviderId` filter)? Decide before building the aggregate tool
behavior.

## 7. Data domain map

| Category | Tables |
|---|---|
| Patient Identity (root) | `Patient` |
| Hospital Infrastructure | `Organization`, `Provider` |
| Hospital / Clinical Care | `Encounter`, `Condition`, `Allergy`, `Careplan`, `Immunization`, `Procedure`, `Device`, `Supply`, `ImagingStudy`, `Medication`, `Observation` |
| Insurance / Payer | `Payer`, `PayerTransition` |
| Billing / Claims (bridge) | `Claim`, `ClaimTransaction` |

Doctor persona (Phase 1) stays mostly within *Hospital/Clinical Care* + *Patient Identity* —
Insurance/Billing domains are out of scope until a later persona phase.

## 8. Model selection

**Chosen: `qwen2.5:7b`** (~4.7 GB). Rationale: the core task (NL question -> valid JSON query
object) is a structured-output task, and Qwen2.5 was specifically trained/benchmarked for
function-calling accuracy (e.g. Berkeley Function-Calling Leaderboard) — not just general chat
quality. At this size class, Llama 3.1:8b was the main alternative (Ollama's own official
tool-calling docs use it) but doesn't have the same structured-output edge.

Smaller Qwen2.5 variants (0.5b/1.5b/3b) were considered and rejected for now — risk is
correctness, not cost: a too-small model produces malformed JSON or misses filters, and the
whole architecture depends on that JSON being valid before it reaches the API.

**Later-phase upgrade path: `command-r`** (Cohere, 35B) — purpose-built for enterprise
RAG/tool-use workflows, a stronger match once the core architecture is proven. Not the starting
point (7x heavier than qwen2.5); layered in after Phase 1 validates the design.

Explicitly rejected for this project: healthcare-domain-fine-tuned models (`meditron`,
`biomistral`) — they optimize for clinical-knowledge QA, a skill this architecture doesn't rely
on (every answer is grounded in tool results, not the model's own medical knowledge), and they
are generally weaker at the strict-JSON/tool-calling task that actually matters here.

## 9. Phased roadmap (beyond Phase 1)

| Phase | What | Data store |
|---|---|---|
| 1 | SQL Server + Ollama + MCP query-DSL tool (this document) | SQL Server (current) |
| 2 | Same functionality, repeated against a different store | Cosmos DB |
| 3 | Introduce vector search | Cosmos DB (+ vector index) |
| 4 | Move to Azure Cloud, build agents | Azure (managed services) |

## 10. Open items (to discuss next)

- Query-generation mechanics: prompt design, how much schema context to give Ollama, whether to
  validate/repair a malformed query object before execution, handling ambiguous questions.
- Doctor-scoping question from §6 (hospital-wide vs. per-provider "recent patients").
- Target hardware confirmation (GPU/VRAM vs. CPU-only) — affects whether 7B stays comfortable.
- MCP server implementation language/SDK choice (leaning C#, reusing existing API conventions —
  not yet finalized).

## References

- Full chronological discussion log (all decisions as they were made, more detail than this
  document): [`hc_agile/worklogs/learn/ai_mcp_ollama/2026-07-16_patient_ai_assistant_plan.md`](../../worklogs/learn/ai_mcp_ollama/2026-07-16_patient_ai_assistant_plan.md)
- Project folder: [`hc_chatbot/README.md`](../../../hc_chatbot/README.md)
- Underlying REST API: [`hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/`](../../../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/)
