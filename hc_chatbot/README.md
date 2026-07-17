# hc_chatbot

Healthcare AI assistant — natural-language patient/insurance lookup on top of the existing
`AI.HealthCare.Patient.API`, using MCP (Model Context Protocol) tools and a locally-run Ollama
LLM. Answers are restricted to what the REST API actually returns from SQL Server — no
fabricated patient data.

**Status: planning stage. No implementation code yet.**

## Architecture (Phase 1)

```
User enters patient name
  -> Ollama (local LLM, tool-calling enabled)
  -> MCP client
  -> MCP server (wraps the existing REST API)
  -> AI.HealthCare.Patient.API (unchanged)
  -> SQL Server
  -> results flow back up
  -> Ollama synthesizes an answer using ONLY the tool results
```

## Phased roadmap

| Phase | What | Data store |
|---|---|---|
| 1 | SQL Server + Ollama + MCP tools wrapping the existing REST API | SQL Server (current) |
| 2 | Same functionality, repeated against a different store | Cosmos DB |
| 3 | Introduce vector search | Cosmos DB (+ vector index) |
| 4 | Move to Azure Cloud, build agents | Azure (managed services) |

Approach: small sample -> demo -> improve -> next phase, one tool/entity at a time.

## Data domains (which table belongs to which "world")

| Category | Tables |
|---|---|
| Patient Identity (root) | `Patient` |
| Hospital Infrastructure | `Organization`, `Provider` |
| Hospital / Clinical Care | `Encounter`, `Condition`, `Allergy`, `Careplan`, `Immunization`, `Procedure`, `Device`, `Supply`, `ImagingStudy`, `Medication`, `Observation` |
| Insurance / Payer | `Payer`, `PayerTransition` |
| Billing / Claims (bridge) | `Claim`, `ClaimTransaction` |

## Reference

- **Technical design document, Option A: fully custom orchestrator** (start here): [`hc_agile/architecture/design_patterns/healthcare_ai_assistant_mcp_ollama_design.md`](../hc_agile/architecture/design_patterns/healthcare_ai_assistant_mcp_ollama_design.md)
- **Technical design document, Option B: ASP.NET Core + Semantic Kernel**: [`hc_agile/architecture/design_patterns/healthcare_ai_assistant_semantic_kernel_design.md`](../hc_agile/architecture/design_patterns/healthcare_ai_assistant_semantic_kernel_design.md) — not yet decided which option Phase 1 uses
- Full chronological discussion log, per-table field inventory, and MCP tool-marking decisions:
  [`hc_agile/worklogs/learn/ai_mcp_ollama/2026-07-16_patient_ai_assistant_plan.md`](../hc_agile/worklogs/learn/ai_mcp_ollama/2026-07-16_patient_ai_assistant_plan.md)
- Underlying REST API this wraps: [`hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/`](../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/)
