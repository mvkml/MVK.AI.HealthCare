# Patient AI Assistant — MCP + Ollama plan (initial discussion)

## The plan (as stated)

Build a natural-language patient lookup: user enters a patient name, and the system returns
associated patient results (conditions, medications, observations, claims, etc. — the 18
Synthea entities already built in `AI.HealthCare.Patient.API`).

Architecture: the existing ASP.NET Core REST API is wrapped as **MCP tools**. **Ollama** runs
locally as the LLM that calls those tools. Critically: **answers must be restricted to what's
actually in the SQL Server database** — the model must not hallucinate patient data.

## Proposed flow

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

## Suggestions / discussion table (2026-07-16)

| # | Aspect | Suggestion | Why |
|---|---|---|---|
| 1 | Overall architecture | Sound — this is "agentic tool-calling," not classic RAG. No vector DB needed since the data is relational/structured; tool calls are the right fit | Matches how the data is actually shaped — a vector DB would be solving a problem that doesn't exist here |
| 2 | MCP server language | Build it in C# using the official MCP C# SDK, reusing existing DTOs/HttpClient/DI conventions from this codebase (same style as `df_chunk_file`) | Avoids introducing a second tech stack just for the MCP layer |
| 3 | Ollama model choice | Must be a tool-calling-capable model (e.g. `llama3.1`, `qwen2.5`, `mistral-nemo`) — not every Ollama model supports function calling | Tool calling is what turns "chat" into "LLM that queries the DB instead of guessing" |
| 4 | Grounding / restriction to DB | Most important decision. System prompt must say: "Only answer using tool call results. If a tool returns nothing, say so. Never state a fact not present in a tool result." | Without this, the LLM will fabricate plausible-sounding medical history — unacceptable for patient data |
| 5 | Scope / phasing | Start with 2–3 tools (`get_patient_by_name`, `get_conditions_by_patient`, `get_medications_by_patient`) before wiring all 18 entities | Proves the tool-calling + grounding loop works before scaling breadth; easier to catch hallucination bugs with a small surface |
| 6 | PHI / security | Decide: does the LLM ever see the full patient list, or only one patient's data after a name lookup narrows it down? | Keeps blast radius small — very different risk profile than an LLM with the entire `Patients` table in context |
| 7 | Orchestration loop | Something must run the ask -> tool request -> call MCP -> feed result back -> answer loop. Decide: small custom script, or an existing agent framework | Most underestimated piece — Ollama gives tool-calling primitives, but the loop itself is still hand-written unless a framework is brought in |
| 8 | Testing | Plan a small "hallucination test suite": ask about a nonexistent patient, ask something tools can't answer, confirm the model says "I don't know" rather than inventing an answer | Directly answers a client's "did you test for this?" question — a strong signal of engineering maturity |

## Client relevance (asked directly)

Yes — this combination is genuinely valuable to discuss with a client in 2026:
- Tool-calling / function-calling with LLMs
- MCP (very new, most candidates haven't touched it)
- Local/on-prem LLM deployment (cost + privacy angle, not "just call OpenAI")
- Explicitly designing against hallucination in a regulated domain (healthcare/PHI) — this is
  the differentiator. Many candidates can wire an LLM to an API; fewer can articulate *why and
  how* they constrained it to avoid fabricating facts about a patient.

## Phased roadmap (added 2026-07-16, same discussion)

Explicit decision: **no vector database in phase 1.** Step-by-step, demo-by-demo progression,
each phase gets a small working sample + demo before moving to the next:

| Phase | What | Data store | Notes |
|---|---|---|---|
| 1 | SQL Server + Ollama + MCP tools wrapping the existing REST API | SQL Server (current) | No vector DB yet. Build the smallest working sample first, turn it into a demo |
| 2 | Same functionality, repeated against a different store | Cosmos DB | Internal/dev exercise — same tool-calling behavior, different backing store, to prove the MCP/Ollama layer isn't coupled to SQL Server specifically |
| 3 | Introduce vector search | Cosmos DB (+ vector index) | Only brought in once the Cosmos DB stage exists — not needed for the structured-data lookups in phase 1 |
| 4 | Move the whole thing to Azure Cloud, build agents | Azure (managed services) | Final stage — agents on top of the proven MCP/tool-calling foundation from earlier phases |

Approach across all phases: small sample -> demo -> improve -> next phase. Prioritize speed —
"demo by demo," not a big upfront design for all four phases at once.

## Full field inventory — all 18 entities (added 2026-07-16)

Every table's complete column list, for designing MCP tool inputs/outputs. All entities below
`Patient` have a `PatientId` foreign key — the natural MCP shape is one `get_patient_by_name`
tool to resolve the Guid, then per-entity `get_<entity>_by_patient` tools (matching the
`GetByPatientId` endpoints already built into every controller).

### Patient
`Id` (Guid, join key), `First`, `Last`, `Middle`, `Prefix`, `Suffix`, `Maiden`, `BirthDate`,
`DeathDate`, `Ssn`, `Drivers`, `Passport`, `Marital`, `Race`, `Ethnicity`, `Gender`, `Birthplace`,
`Address`, `City`, `State`, `County`, `Fips`, `Zip`, `Lat`, `Lon`, `HealthcareExpenses`,
`HealthcareCoverage`, `Income`.
Primary search fields: `First`, `Last`, `BirthDate`, `Ssn`.

### Organization
`Id`, `Name`, `Address`, `City`, `State`, `Zip`, `Phone`, `Lat`, `Lon`, `Revenue`, `Utilization`.
Primary search fields: `Name`, `City`, `State`.

### Provider
`Id`, `OrganizationId` (FK), `Name`, `Gender`, `Speciality`, `Address`, `City`, `State`, `Zip`,
`Lat`, `Lon`, `Encounters`, `Procedures`.
Primary search fields: `Name`, `Speciality`.

### Payer
`Id`, `Name`, `Ownership`, `Address`, `City`, `StateHeadquartered`, `Zip`, `Phone`,
`AmountCovered`, `AmountUncovered`, `Revenue`, `CoveredEncounters`, `UncoveredEncounters`,
`CoveredMedications`, `UncoveredMedications`, `CoveredProcedures`, `UncoveredProcedures`,
`CoveredImmunizations`, `UncoveredImmunizations`, `UniqueCustomers`, `QolsAvg`, `MemberMonths`.
Primary search field: `Name`.

### PayerTransition
`Id` (long), `PatientId` (FK), `MemberId`, `StartDate`, `EndDate`, `PayerId` (FK),
`SecondaryPayerId` (FK), `PlanOwnership`, `OwnerName`.
Primary search fields: `PatientId`, `StartDate`/`EndDate`.

### Encounter
`Id`, `Start`, `Stop`, `PatientId` (FK), `OrganizationId` (FK), `ProviderId` (FK), `PayerId` (FK),
`EncounterClass`, `Code`, `Description`, `BaseEncounterCost`, `TotalClaimCost`, `PayerCoverage`,
`ReasonCode`, `ReasonDescription`.
Primary search fields: `PatientId`, `EncounterClass`, `Description`, `Start`/`Stop`.

### Condition
`Id` (long), `Start`, `Stop`, `PatientId` (FK), `EncounterId` (FK), `System`, `Code`,
`Description`.
Primary search fields: `PatientId`, `Code`/`Description`, `Start`/`Stop` (active vs. resolved).

### Allergy
`Id` (long), `Start`, `Stop`, `PatientId` (FK), `EncounterId` (FK), `Code`, `System`,
`Description`, `Type`, `Category`, `Reaction1`, `Description1`, `Severity1`, `Reaction2`,
`Description2`, `Severity2`.
Primary search fields: `PatientId`, `Description`, `Category`, `Reaction1`/`Reaction2`.

### Medication
`Id` (long), `Start`, `Stop`, `PatientId` (FK), `PayerId` (FK), `EncounterId` (FK), `Code`,
`Description`, `BaseCost`, `PayerCoverage`, `TotalCost`, `Dispenses`, `ReasonCode`,
`ReasonDescription`.
Primary search fields: `PatientId`, `Description`, `Start`/`Stop`, `ReasonDescription`.

### Careplan
`Id`, `Start`, `Stop`, `PatientId` (FK), `EncounterId` (FK), `Code`, `Description`, `ReasonCode`,
`ReasonDescription`.
Primary search fields: `PatientId`, `Description`, `ReasonDescription`.

### Procedure
`Id` (long), `Start`, `Stop`, `PatientId` (FK), `EncounterId` (FK), `System`, `Code`,
`Description`, `BaseCost`, `ReasonCode`, `ReasonDescription`.
Primary search fields: `PatientId`, `Description`, `Start`.

### Immunization
`Id` (long), `Date`, `PatientId` (FK), `EncounterId` (FK), `Code`, `Description`, `BaseCost`.
Primary search fields: `PatientId`, `Description`, `Date`.

### Device
`Id` (long), `Start`, `Stop`, `PatientId` (FK), `EncounterId` (FK), `Code`, `Description`, `Udi`.
Primary search fields: `PatientId`, `Description`.

### Supply
`Id` (long), `Date`, `PatientId` (FK), `EncounterId` (FK), `Code`, `Description`, `Quantity`.
Primary search fields: `PatientId`, `Description`.

### ImagingStudy
`Id` (long), `StudyId` (Guid), `Date`, `PatientId` (FK), `EncounterId` (FK), `SeriesUid`,
`BodysiteCode`, `BodysiteDescription`, `ModalityCode`, `ModalityDescription`, `InstanceUid`,
`SopCode`, `SopDescription`, `ProcedureCode`.
Primary search fields: `PatientId`, `BodysiteDescription`, `ModalityDescription`, `Date`.

### Observation
`Id` (long), `Date`, `PatientId` (FK), `EncounterId` (FK, nullable), `Category`, `Code`,
`Description`, `Value`, `Units`, `Type`.
Primary search fields: `PatientId`, `Code`/`Description`, `Value`/`Units`, `Date`, `Category`.
Note: this is where vitals/lab results live — likely one of the most common query targets.

### Claim
`Id`, `PatientId` (FK), `ProviderId` (FK), `PrimaryPatientInsuranceId`,
`SecondaryPatientInsuranceId`, `DepartmentId`, `PatientDepartmentId`, `Diagnosis1`-`Diagnosis8`,
`ReferringProviderId`, `SupervisingProviderId`, `AppointmentId`, `CurrentIllnessDate`,
`ServiceDate`, `Status1`, `Status2`, `StatusP`, `Outstanding1`, `Outstanding2`, `OutstandingP`,
`LastBilledDate1`, `LastBilledDate2`, `LastBilledDateP`, `HealthcareClaimTypeId1`,
`HealthcareClaimTypeId2`.
Primary search fields: `PatientId`, `Diagnosis1`-`8`, `ServiceDate`, `Status1`/`2`/`P`.

### ClaimTransaction
`Id`, `ClaimId` (FK), `ChargeId`, `PatientId` (FK), `Type`, `Amount`, `Method`, `FromDate`,
`ToDate`, `PlaceOfServiceId`, `ProcedureCode`, `Modifier1`, `Modifier2`, `DiagnosisRef1`-`4`,
`Units`, `DepartmentId`, `Notes`, `UnitAmount`, `TransferOutId`, `TransferType`, `Payments`,
`Adjustments`, `Transfers`, `Outstanding`, `AppointmentId`, `LineNote`, `PatientInsuranceId`,
`FeeScheduleId`, `ProviderId`, `SupervisingProviderId`.
Primary search fields: `PatientId`, `ClaimId`, `Type`, `Amount`, `ProcedureCode`, `Notes`.

## Persona-first approach (added 2026-07-16)

Decision: build Phase 1 for **one persona first — the doctor** — before other personas
(patient, insurance). Doctor-facing questions fall into two different query shapes, which
matters for tool design:

| Query type | Example | Shape |
|---|---|---|
| Single-patient lookup | "Patient details for John Smith" | Search by name -> one patient's record |
| Aggregate / list query | "How many patients are there", "recent visiting patients" | Scans across patients/encounters, not tied to one name |

Open question (not yet resolved): does "recent visiting patients" mean hospital-wide, or scoped
to the specific doctor's own patients (filtered by `ProviderId`)? Revisit when building.

## LOCKED ARCHITECTURE DECISION — dynamic query generation, not fixed per-question tools
### (added 2026-07-16 — reference this for demo/PPT prep)

**The problem:** a doctor can ask virtually anything ("patients on medication X", "patients
over 50 with condition Y", "recent visits by Dr. Z"...). Pre-writing a fixed REST
endpoint/MCP-tool per possible question is combinatorial and unbounded — not feasible.

**The decision:** instead of many fixed tools, build **one generic AI-search endpoint** on the
ASP.NET Core API. Ollama's job is to turn the natural-language question into a **query**,
based on the database schema (the full field inventory documented earlier in this file), and
send that query to the endpoint (exposed as a single MCP tool, e.g. `execute_healthcare_query`).
The endpoint executes it and returns JSON.

**The critical sub-decision — what "query" means:**

| Approach | How it works | Risk |
|---|---|---|
| ❌ Ollama generates raw SQL text; API executes it directly | Simplest to build | Dangerous — classic SQL-injection-by-LLM. Even a "trusted" model can hallucinate a destructive or wrongly-scoped query |
| ✅ **Chosen**: Ollama generates a structured JSON query object; API validates + translates to safe, parameterized EF Core LINQ | Slightly more upfront work | Safe — API only ever executes LINQ it built itself. Can enforce read-only, allow-listed tables/columns only, no destructive operation possible even in principle |

**Structured query DSL — example shapes** (minimal version, to be expanded over time):

Recent top-10 visiting patients:
```json
{
  "table": "Encounter",
  "select": ["PatientId", "Start", "Description", "ReasonDescription"],
  "orderBy": { "field": "Start", "direction": "desc" },
  "limit": 10
}
```

Patient details by name:
```json
{
  "table": "Patient",
  "filters": [
    { "field": "First", "op": "eq", "value": "John" },
    { "field": "Last", "op": "eq", "value": "Smith" }
  ]
}
```

**Why this is safe and scalable:**
- Ollama never writes SQL — only fills in this constrained JSON shape.
- The field inventory already documented in this file becomes **dual-purpose**: (a) the schema
  context given to Ollama so it knows what tables/fields exist, and (b) the API's server-side
  allow-list — any `table`/`field` name not in the inventory is rejected before any query runs.
- The API enforces read-only (SELECT-equivalent only) at the translation layer — there is no
  code path from this endpoint to INSERT/UPDATE/DELETE.
- Start the DSL minimal (equality filters + `orderBy` + `limit` — covers both example doctor
  questions above) and expand expressiveness incrementally (more operators: `contains`,
  `gt`/`lt`, `between`, joins across tables) as new question types come up in later demos —
  keeps the "demo by demo" iteration model even for the query engine itself.

**Open discussion item (explicitly flagged, to continue next)**: the mechanics of *how* Ollama
reliably generates a correct query from a natural-language question — prompt design, how much
schema context to include, whether to validate/repair malformed query objects before execution,
and how to handle ambiguous questions. This is called out as its own discussion topic to pick up
next.

## Ollama model shortlist (added 2026-07-16)

All Ollama models are free (open-weight, run locally, no API cost) — the real filter for this
project isn't "free," it's **which ones actually support tool-calling/function-calling**, since
many general chat models on Ollama don't. Shortlist, sized by hardware requirement:

| Model | Params | Download size | Tool-calling? | Best for | Notes |
|---|---|---|---|---|---|
| **`qwen2.5:7b`** | 7B | ~4.7 GB | Strong | **Recommended starting point** | Best balance of small size + reliable structured JSON output — matches the query-DSL approach's needs |
| `llama3.1:8b` | 8B | ~4.9 GB | Native | Solid alternative | Meta's official tool-calling format, well documented for Ollama specifically |
| `mistral-nemo:12b` | 12B | ~7.1 GB | Good | If 7-8B proves unreliable | Slightly heavier, better reasoning step up |
| `qwen2.5:14b` | 14B | ~9.0 GB | Strong | Better reasoning for complex/ambiguous questions | Only if hardware has the RAM/VRAM |
| `granite3.1-dense:8b` (IBM) | 8B (2B variant ~1.6 GB also exists) | ~4.9 GB | Yes | Lightweight, enterprise-tuned fallback | Good if 7-8B models are still too heavy |
| `llama3.3:70b` | 70B | ~43 GB | Best quality | Only with serious GPU (~40GB+ VRAM) | Overkill for a demo, noted for completeness |

Sizes are standard default-quantization (Q4) download sizes from the Ollama library, not
independently re-verified by pulling each model. Running memory needed is roughly 1.2-1.5x the
download size (e.g. `qwen2.5:7b` at 4.7 GB needs ~6-8 GB RAM/VRAM free to run comfortably).

Checked locally installed models (`ollama list`) as of 2026-07-16: `llama3:latest` (4.7 GB),
`llama3.2:latest` (2.0 GB), `codegemma:2b` (1.6 GB), `viki:latest` (4.7 GB) — none of the
shortlisted tool-calling models above are pulled yet.

**Recommendation: start with `qwen2.5:7b`** — specifically known for strong structured/JSON
output relative to its size, which is exactly the skill the query-DSL approach depends on
(reliably filling in the `{"table": ..., "filters": [...]}` shape without malformed JSON).

Open item: confirm target hardware (GPU/VRAM vs. CPU-only) to finalize whether 7-8B is
comfortable or a smaller/larger model fits better.

### Why `qwen2.5:7b` over the alternatives (demo/PPT justification, added 2026-07-16)

**One-line pitch**: our core task — turning a doctor's question into a strict JSON query
object — is fundamentally a structured-output task, and Qwen2.5 was specifically trained and
benchmarked for function-calling/structured-output accuracy (e.g. Berkeley Function-Calling
Leaderboard), not just general chat quality.

| Comparison point | Qwen2.5:7b | Llama 3.1:8b | Why it matters here |
|---|---|---|---|
| Function-calling / structured output | Purpose-built and benchmarked for it | Supports tool-calling, but general-purpose, not specifically optimized for it | Reliable valid-JSON output matching our query DSL schema is the single most important quality metric for this project |
| Size / footprint | 4.7 GB, ~7B params | 4.9 GB, ~8B params | Nearly identical — not the deciding factor |
| Cost | Free, open-weight, local | Free, open-weight, local | Equal — capability is the differentiator, not cost |
| Ecosystem support | Well supported | Ollama's own official tool-calling examples use this model | Llama 3.1 is the safer fallback/reference if Qwen2.5 underperforms on our specific schema in testing |
| Why not 14B/70B? | — | — | Diminishing returns — structured JSON generation for a modest schema doesn't need 70B-scale general reasoning; bigger = slower + heavier hardware with no accuracy gain for this narrow task |

**Narrative order for the demo/PPT:**
1. The task is narrow (NL question -> valid JSON query object), not open-ended chat — pick the
   model benchmarked for that specific skill, not the most "powerful" overall.
2. Qwen2.5 wins that specific comparison against same-size peers.
3. Llama 3.1 stays as the documented fallback/reference, since it's the model Ollama's own
   tool-calling docs are built around.
4. Bigger models (14B-70B) were considered and explicitly rejected for this phase — the
   cost/speed tradeoff isn't justified when the task doesn't require that much general
   reasoning power.

### Enterprise / healthcare-domain model comparison (added 2026-07-16)

Distinguished two different meanings of "best enterprise model for healthcare/insurance":

| Model | Category | Size | Tool-calling | Fit |
|---|---|---|---|---|
| `command-r` (Cohere) | Enterprise RAG/tool-use | 35B | Strong — purpose-built for it | Designed specifically for grounded enterprise workflows (retrieve -> cite -> answer), close match to our architecture. Heavier than qwen2.5 |
| `granite3.1-dense` (IBM) | Enterprise general-purpose | 2B/8B | Yes | IBM markets around enterprise governance/traceability/safety; strong healthcare-industry brand credibility (Watson Health legacy) |
| `meditron:70b`, `biomistral` | Healthcare-domain fine-tuned | 70B / 7B | Weak/unclear | Fine-tuned for clinical-knowledge QA, not for structured tool-calling — **not actually the right fit** for this architecture, see below |
| `qwen2.5:7b` | General, tool-calling optimized | 7B | Strong | Lightest footprint, still best fit for the mechanical query-generation task |

**Key correction captured**: a medically-fine-tuned model (Meditron, BioMistral) is not the
right tool here, and may be worse. This architecture grounds every answer in SQL data returned
by a tool call, so the model never needs to "know medicine" itself — it only needs to (1)
understand the question, (2) generate a correct structured query, (3) phrase the DB result back
in plain language. Medical fine-tuning optimizes for a skill deliberately not relied upon here,
and doesn't help — and these models are generally weaker at the strict-JSON/tool-calling task
that actually matters.

### Low-cost framing (added 2026-07-16)

Since Ollama models are free/local, "low cost" = least hardware/compute needed, not a price
comparison. Smaller Qwen2.5 variants exist (`0.5b` ~0.4GB, `1.5b` ~1GB, `3b` ~2GB) but the risk
of going smaller than 7B is correctness, not cost: too-small a model produces malformed JSON or
misses filters, and the whole architecture depends on that JSON being valid before it reaches
the API. `qwen2.5:7b` remains the recommendation — reliability matters more than shaving a
couple GB, unless hardware genuinely can't run ~5GB comfortably (hardware specs still an open
item to confirm).

### FINAL CONFIRMED DECISION (2026-07-16): starting model = `qwen2.5:7b`

Confirmed — implementation work begins with **`qwen2.5:7b`**. `command-r` remains the planned
later-phase upgrade once the core architecture is proven (see phased rollout below).

### LOCKED DECISION (2026-07-16): phased model rollout

- **Phase 1 (now): `qwen2.5:7b`** — lightweight, strong structured-output/tool-calling for its
  size, fastest to stand up the first working demo.
- **Later phase: incorporate `command-r`** (Cohere) — once the core architecture (query DSL,
  MCP tool, grounding) is proven with qwen2.5, layer in Command R for its purpose-built
  enterprise RAG/tool-use behavior. Command R is the "enterprise-grade" upgrade path, not the
  starting point — heavier (35B) but a stronger match to enterprise-grounded workflows once
  the basics are validated.

## Multi-agent pipeline design (added 2026-07-16)

Full agent-pipeline architecture proposed for the query-handling flow:

| # | Agent | Role | Input | Output | Trigger |
|---|---|---|---|---|---|
| 1 | Master / Request Agent | Entry point — receives the raw user question, orchestrates the pipeline | User's question | Routes to Agent 2 | Always |
| 2 | Query-Check (Classifier) Agent | Decides whether the question needs a database query at all | Question | `needs_query: true/false` | Always |
| 3 | Information Agent | Fallback for non-DB questions — answers from web search or another LLM (e.g. Claude) | Question | Plain-language answer | Only if Agent 2 = false |
| 4 | Query Builder Agent | Builds the structured JSON query DSL from the question (per the locked query-DSL design) | Question + schema | Query DSL JSON | Only if Agent 2 = true |
| 5 | Query Validation / Guardrail Agent | Checks for hallucination, confirms read-only (no delete/update/insert possible), enforces record-count limits (default 10/100, hard cap e.g. 100,000) | Query DSL JSON | Approved query or rejection | After Agent 4 |
| 6 | Query Execution Agent | Sends the validated query to the REST API — the actual MCP tool call | Approved query | Raw JSON | After Agent 5 approves |
| 7 | Response Formatting Agent | Converts raw JSON into clean, structured information | Raw JSON | Structured result | After Agent 6 |
| 8 | Consolidation Agent | Merges the structured DB result with any supplementary web info, if both present | Structured result (+ web info) | Unified response | Always |
| 9 | Marketing / Contextual-Branding Agent (final) | Wraps the final answer with client/hospital-specific context (e.g. "please contact [Hospital] for more information") — keeps responses scoped to the specific client's identity | Unified response | Final answer to user | Always, last step |

**Flow**: Master -> Classifier -> (Information Agent *or* Builder -> Validator -> Executor ->
Formatter) -> Consolidation -> Branding -> shown to user.

**Rating given (out of 10, for initial-step/Phase-1 purposes): 6/10.** Design quality alone is
strong (~8-9/10) — good separation of concerns, and Agent 5 (hallucination check + read-only
enforcement + record-count caps) is exactly the right safety discipline for this project, not
overkill. Rating lowered for *initial-step* practicality: 9 sequential stages is a lot for a
first demo — if each is a separate Ollama call, that's up to 9 LLM round-trips per question,
meaning real latency and 9x the failure surface while the core loop is still unproven.

**Suggestion (not yet decided/locked)**: collapse to 3 stages for the first working demo
(Classify -> Build+Validate combined -> Execute+Format+Brand combined), prove the loop works
end-to-end, then split into the full 9-agent pipeline once the basics are solid. Same
destination, lower-risk path there. To be discussed further before locking.

### CONFIRMED (2026-07-16): implement one agent at a time, starting with a single MVP agent

Explicit instruction: implement the 9-agent pipeline incrementally, not all at once. First
build a single consolidated **Agent v0**:
1. Accept the user's request (natural-language question)
2. If it requires a query -> build the query (structured JSON DSL)
3. Call the tool (MCP tool -> REST API -> SQL Server)
4. Return the response in a neat, structured format

This covers Agents 1, 2 (implicitly), 4, 6, and 7 from the full pipeline in one pass. Deferred
for later, added one at a time once this loop works end-to-end: Information Agent (Agent 3,
non-DB fallback), separate Validation/Guardrail agent (Agent 5), Consolidation (Agent 8),
Marketing/Branding wrapper (Agent 9).

**Open question before building**: for this first version, should the "if anything queries"
check be a real classify step (query vs. non-query), or should every input be assumed to be a
query for now, with the Information Agent branch added back in later? Not yet resolved.

## Agent implementation approach in ASP.NET Core — Foundry coupling question (added 2026-07-16)

Question raised: is building agents in .NET necessarily tied to Microsoft's Agent SDK / Azure AI
Foundry Agent Service, or can a fully custom agent be built instead?

**Answer: only Azure AI Foundry Agent Service itself is Azure/Foundry-coupled** (managed cloud
service, tied to Azure resources, Azure-OpenAI-flavored). That is not the only option in .NET:

| Option | Coupled to Foundry/Azure? | What it is |
|---|---|---|
| Azure AI Foundry Agent Service | Yes — cloud-hosted, Azure-tied | Skip for Phase 1-3 |
| Fully custom, hand-rolled orchestrator | No dependency at all | Plain C# class + `HttpClient` calling Ollama's `/api/chat` endpoint directly (natively supports function/tool-calling); parse tool-call response, execute the tool (call the REST API), feed result back, loop |
| Semantic Kernel (Microsoft, open-source) | Not Foundry-locked | .NET orchestration library — `Kernel`, tool/plugin registration, conversation-loop management. Works with Ollama (OpenAI-compatible endpoint or community connector). Runs fully local/on-prem |
| Microsoft.Extensions.AI | Not Foundry-locked | Newer, lighter common-interface abstraction across AI providers, also supports Ollama + function-calling |

**Decision**: yes, a fully custom agent in ASP.NET Core is possible and is the recommended
approach for Phase 1 — a hand-rolled orchestrator calling Ollama + the existing REST API
directly, with nothing Foundry-related in it. This fits the roadmap better than Foundry would
right now, since Phases 1-3 are explicitly local/on-prem; Foundry only becomes a *possible*
(not required) option at Phase 4 once the system moves to Azure Cloud.

**Recommended for Phase 1**: fully custom, hand-rolled orchestrator (no Semantic Kernel, no
Foundry) — most direct match to the 9-agent pipeline already designed (each agent = one C#
method/class in a chain), full control, zero external SDK risk while the core loop is still
being proven.

## Third orchestration option: LangGraph + FastAPI (added 2026-07-16)

Raised as a candidate alongside Option A (custom C#) and Option B (Semantic Kernel, see
`healthcare_ai_assistant_semantic_kernel_design.md`): **LangGraph** (Python, by the LangChain
team) is purpose-built for stateful multi-agent graphs — nodes + conditional edges — which is
arguably the best conceptual match for the exact 9-agent pipeline already designed. Would run
as a separate **FastAPI** (Python) service, calling the existing ASP.NET Core Patient API as its
tool over HTTP — same relationship MCP would have, different runtime hosting the orchestrator.

| | Option A: Custom C# | Option B: Semantic Kernel (C#) | Option C: LangGraph + FastAPI (Python) |
|---|---|---|---|
| Fit for the exact 9-agent graph pipeline | Good — hand-wired chain of C# classes | Decent — function-calling loop, not graph-native | Best — this is literally what LangGraph models natively |
| Ollama support maturity | Direct HTTP, always current | Experimental connector | Mature — `langchain-ollama` is first-class |
| Tech stack | Same as rest of the codebase | Same as rest of the codebase | A second stack — new runtime, deployment, dependency ecosystem |
| Repo consistency | Reuses existing DI/config/logging patterns | Reuses existing patterns | Breaks pattern — same tension as the earlier "MCP server in C# vs Node" decision |
| Ecosystem/community size | N/A (hand-rolled) | Smaller, Microsoft-only | Much larger — most agent tutorials/examples are LangChain/LangGraph |

Middle option noted: **LangGraph.js** exists too — avoids Python specifically, reuses the
TypeScript already present in this repo (Playwright suite), though still a separate Node
runtime alongside ASP.NET Core, not a true single stack.

Not yet decided between Option A/B/C — three-way fork now open, to be resolved before
implementation starts.

### Job-market comparison of the three options (added 2026-07-16)

| | Option A: Custom C# | Option B: Semantic Kernel (C#) | Option C: LangGraph + FastAPI (Python) |
|---|---|---|---|
| Job market demand (2026) | Broad but generic — value comes from "C#/.NET" broadly, not this specific choice | Growing but niche — mostly Microsoft-stack enterprises | Highest — LangChain/LangGraph are among the most frequently listed keywords in AI/LLM/agentic-AI job postings |
| Where most valued | Traditional enterprise backend (banking, insurance, healthcare) | .NET shops investing in Copilot/Azure AI | Broadest — AI/ML roles across nearly every industry |
| Resume/ATS keyword value | Low as a standalone line | Medium — differentiates among .NET devs | High — actively filtered for by recruiters/ATS |
| Client discussion value | Strong on fundamentals depth | Decent — Microsoft-ecosystem fluency | Strong and broad — dominant AI-tooling stack |
| Breadth of roles opened | Narrower — .NET/enterprise backend | Narrower — Microsoft-stack AI roles | Widest — startups, AI-first companies, and enterprises all use this |

**Conclusion drawn**: for job-market purposes specifically, Option C (LangGraph + FastAPI) has
the clearest edge — it's the stack most AI-focused job postings actually name. Options A/B are
strong for the current domain (healthcare, which is C#-heavy here) but less of a resume
differentiator. Noted separately: having evaluated and documented tradeoffs across all three
options is itself a strong artifact for client discussion, regardless of which is ultimately implemented.

## Status

Discussion-stage only — no code written yet. Architecture direction (structured query DSL,
persona-first with doctor) is locked for demo/PPT reference. Plan is to go through the 8 points
above one by one for Phase 1 before implementing, plus the flagged open discussion items on
query-generation mechanics, target hardware/model sizing, and the three-way orchestration
option fork (custom C# vs. Semantic Kernel vs. LangGraph+FastAPI).
