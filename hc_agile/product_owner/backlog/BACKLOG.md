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
| PB019 | Database-backed LLM/model selection by persona/user-type (Doctor, Insurance Provider, Client) — `ILLMModelBL.GetModelDetails` should look up provider (Ollama, OpenAI, etc.) + model per persona from a config table instead of always returning the single appsettings-bound Ollama config | Medium | To Do |
| PB020 | Wire the actual `LLM -> TOOL -> BL` query-DSL path (`HealthcareQueryTool`, database-grounded answers) into the Doctor flow, and give the Guardrail safety requirement (allow-list validation, read-only enforcement, §4 of the design doc) a home in the diagram | High | To Do |
| PB021 | Demo 2: Classification-driven routing — classify each request (via an "HC Classification" model) before generating a response, then use a Factory to dispatch to the right prompt/handling based on the classification | High | Discussion — gathering requirements, not yet designed |
| PB022 | Authentication: Login/Signup/Home (US009) — Angular UI built mock-first, referencing `ai_hr`'s `hr_ui/aihrweb` + `AI.HR.Api` as a structural pattern | High | Done (UI only) |
| PB023 | Real auth backend — schema, persona/role model, JWT vs. session decision, token-storage security review, where the API lives (`HC.AI.MAPI` vs. standalone) | High | DB + API done (`HC.AI.Identity.Api`, merged into `AI_HealthCarePatient`); Angular wiring is PB025 |
| PB024 | Chat UI for Patient persona (Angular, US010) — separate page from Doctor chat, shared presentational components, mock-only (no Patient backend exists) | Medium | Done (UI only) |
| PB025 | Wire Angular `AuthMockService` (US009) to the real `HC.AI.Identity.Api` login/signup/forgot-password/reset-password endpoints, replacing the `localStorage`-only mock; token-storage approach (`localStorage` vs httpOnly cookie) still needs explicit sign-off | High | Done (login/signup only — forgot/reset-password deferred, see TASK013) |
| PB026 | Rename `AI.HealthCare.Patient.API` (`hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API`) to `HC.AI.Patient.API` — matches the `HC.AI.*` family already used by `HC.AI.MAPI` and `HC.AI.Identity.Api`; cascades to the sibling `.BL`/`.EF`/`.Models`/`.Repositories`/`.Tests` projects, namespaces, `.csproj`/`.sln` refs | Medium | To Do — not started, logged per user request |
| PB027 | Set up Azure DevOps process for client/demo showcase — project **MVK AI Health Care** (Basic plan, free ≤5 users), mirror `hc_agile/product_owner/backlog/BACKLOG.md` (PB001–PB026) into Boards as Epics/Features/User Stories/Tasks/Test Cases | High | In Progress — PAT connected and verified; one generic Epic #1 ("Azure DevOps Environment Setup") + Issue #2 ("Set up the environment") created, but these don't map to any PB item and don't yet reflect the mirroring scope in TASK014 — see notes |
| PB028 | Sequence diagram + technical design flow docs for `aihcweb` (Angular UI) — text flow charts in the ADO Wiki, kept in sync as the app changes | Medium | In Progress — Architecture-1 (structure/routing snapshot) done 2026-07-20; sequence-diagram-specific content (login/signup/chat request flows over time) not yet written |
| PB029 | Sequence diagram + technical design flow docs for `HC.AI.MAPI` (Doctor persona REST API) — text flow charts in the ADO Wiki, kept in sync as the app changes | Medium | In Progress — Architecture-1 (layer structure + active request path + gaps) done 2026-07-20; sequence-diagram-specific content not yet written |
| PB030 | Sequence diagram + technical design flow docs for `HC.AI.Identity.Api` (Auth REST API) — text flow charts in the ADO Wiki, kept in sync as the app changes | Medium | In Progress — Architecture-1 (layer structure + active request path + gaps) done 2026-07-20; sequence-diagram-specific content not yet written |
| PB031 | Admin Identity — Login + Sign-Up pages (Angular, `/admin/...` routes), mock-first (no backend Admin role exists yet) | High | Done (mock) — see TASK015; mirrored to ADO as Epic #44 > Feature #45 > User Stories #46/#47 |
| PB032 | EPIC001: Dynamic persona-driven LLM + prompt resolution — DB-backed Classification/Executor model and prompt selection per persona, replacing static appsettings config; supersedes/expands PB019 | High | Mechanism implemented with mock config models (TASK017); real DB-backed version still blocked on schema sign-off. See [`EPIC001`](../epics/EPIC001_dynamic_persona_llm_prompt_resolution.md), US011-US013 |
| PB033 | `HC.AI.Admin.API` (.NET) — dedicated ASP.NET Core REST API for Admin functionality: signup + login, backed by a new `Admins` table in `AI_HealthCarePatient` | High | Done — 2026-07-20, see US016/US020/TASK016/TASK018. ADO sync deferred per user instruction |
| PB034 | Every prompt request must explicitly set/carry a persona, and downstream decisions (model selection, routing, prompt choice) must branch on that persona value — not fall through to a single default. Overlaps `PB019`/`PB032`/`EPIC001`; this item specifically flags the *request-time persona requirement* as its own high-priority piece | High | Model-selection branching done (TASK019) for the Doctor/Patient split at the config (appsettings) level — live-verified both personas resolve to different models. DB-backed version (PB019/PB032) still open; Admin-persona coverage explicitly descoped, see PB035 |
| PB035 | Admin persona chat/prompt capability — Admin does not chat with an LLM (unlike Doctor/Patient); no prompt/LLM path needed for `HC.AI.Admin.API` | Low | Deferred — user explicitly descoped this 2026-07-21; revisit only if that changes. Current focus is Doctor + Patient only |

## Notes
- PB001 is the only item with verified work: PySpark environment is confirmed working and the `allergies` Synthea table has been read and profiled. 18 more Synthea tables (patients, encounters, conditions, medications, procedures, claims, etc.) are present in `hc_bigdata/data/patient_details/synthea/` and not yet explored.
- PB002 exists because `hc_bigdata` currently carries copy-pasted HR-project residue: `data/employees.csv` (HR dummy data), setup docs referencing the `hr_bigdata` app name, and `.claude/settings.json` permissions pointing at `ai_hr\hr_bigdata` paths. Clean up before building further on top of it.
- PB003–PB007 all have folder/project scaffolding in place (layered .NET solutions, Angular app shell, Playwright fixtures/tests dirs) but **zero implementation files** — these are "To Do" from scratch, not partially done.
- PB008–PB010 have empty directory skeletons only, with no defined scope yet.
- PB013 raised by Dev SQL Agent. User specified: LLM Tool Layer sends a table/query shape + a max-records parameter (default 10), and the query needs to support joins across patient-related tables, decided dynamically at runtime since the LLM's exact query shape isn't known in advance. First version implemented in `hc_data_source/hc_sql/` (`usp_ExecuteHealthcareQuery` + whitelist/audit tables) as a **structured, whitelist-validated** dynamic query, not raw dynamic SQL text — see PB014 for why raw SQL was not used.
- PB014 exists because the user's request as stated ("dynamic query", "whatever the LLM builds", raw query text + provider type) is the literal ❌ option in `healthcare_ai_assistant_mcp_ollama_design.md` §4's risk table (LLM-generates-SQL -> SQL-injection-by-LLM). Implemented PB013 using the ✅ option instead (whitelisted table/column/join metadata + parameterized `sp_executesql`), but this needs an explicit ADR so the decision is recorded, not just implied by the code.
- PB016's "provider type" language from the original ask most likely maps to the already-open scoping question in the design doc §6/§10 (is "recent patients" hospital-wide or scoped to the asking doctor's own `ProviderId`?) — `usp_ExecuteHealthcareQuery` accepts an optional `@ProviderId` to support either behavior, but which one is the *default* is a product decision, not a SQL one.
- PB019 raised when wiring `DoctorService` to fetch `LLMOptions` via a proper BL call instead of reading config directly — `HC.AI.MAPI.BL.LLMModel.ILLMModelBL`/`LLMModelBL` now exists and every caller already asks it "which model for this persona," but the implementation is a stub: it always returns the Ollama config from appsettings, ignoring `PromptItem.Persona`. Swapping in the real DB-backed lookup later is scoped to this one class.
- PB021 raised by the user starting Demo 2 planning — see
  `hc_agile/worklogs/dev_semantic_kernel/20260719_170621_demo2_classification_discussion.md` for
  the full discussion-stage understanding and open items. Do not implement until the user provides
  the remaining detail.
- PB020 raised by Architect review of Module 1 (`DoctorController -> ... -> DoctorSemanticProcess`, see `hc_agile/worklogs/dev_semantic_kernel/20260718_180408_module1_doctor_prompt_locked.md`): that module proves the AI-calling plumbing end-to-end, but never calls `HealthcareQueryTool` or reaches the database — it does not yet deliver the actual product feature from `healthcare_ai_assistant_mcp_ollama_design.md` (natural-language lookups grounded only in real query results). The design doc's §4 safety requirement (server-side allow-list validation, read-only enforcement) was flagged as an open gap back on 2026-07-17 (`2026-07-17_service_layer_architecture_discussion.md`) and still has no home in the `SL -> AL -> ...` diagram. Do not present Module 1 to a client as "the patient-query assistant" — it's the infrastructure layer underneath that feature, not the feature itself.
- PB022/PB023 split follows the same pattern as US008/US007: UI (PB022) is unblocked and mock-first;
  real backend (PB023) is blocked on the user providing the auth DB schema, plus Architect
  decisions on persona/role model, where the API lives, and JWT-in-localStorage vs. httpOnly
  cookies (the `ai_hr` reference uses `localStorage`, not yet confirmed as acceptable here). Full
  13-step plan (including the still-unconfirmed "does a route guard exist in the ai_hr reference"
  item) captured in the Dev Angular worklog for US009 before any code was written.
- PB023 update (2026-07-19): the user pointed to an existing, already-implemented ASP.NET Core
  auth API at `hc_ai_in/mapi/AI.HR.Api` (untracked in git) — full signup/login/forgot-password/
  reset-password/roles, JWT issuing, EF Core schema (`Users`/`Roles`/`OcrDocuments`), previously
  running against its own `AI_HR` LocalDB database. Per user instruction ("one database", "do not
  delete any source related database tables"), the schema was added to `AI_HealthCarePatient`
  (Dev SQL Agent) via the existing `AI.HR.EF` migrations — additive only, no existing tables
  touched — `AI.HR.Api`'s connection string repointed there, and the 9 existing user rows + 2
  OcrDocuments rows copied over (IDs preserved). The original `AI_HR` database was left completely
  untouched (read-only source), per instruction. Verified live: build succeeds, signup + login
  smoke-tested end-to-end against `AI_HealthCarePatient`, JWT issued correctly. See
  `hc_agile/worklogs/dev_dotnet/` and `hc_agile/worklogs/dev_sql/` for the full session writeup.
- PB023 update (2026-07-19, same day): user resolved the persona/role open item directly —
  `Roles` simplified from the 7 HR-domain names to just `1 Doctor` / `2 Patient` via migration
  `SimplifyRolesToDoctorPatient`. The 8 existing test/demo users referencing the removed roles
  (3-7) were reassigned to Patient by default before the migration ran (FK constraint required it);
  1 user that already had RoleId=1 is now Doctor by inheritance, not deliberate choice. User
  explicitly flagged this as a starting point, to be refined later. `UserBL.DefaultSignUpRoleId`
  updated to `2` (Patient) for self-service signup.
- PB025 raised immediately after PB023's backend became real — Dev Angular Agent's `AuthMockService`
  (`hc_ui/aihcweb/src/app/features/auth/data/auth-mock.service.ts`) currently simulates login/signup
  entirely client-side; it needs to call the real backend endpoints instead. See TASK013.
- PB023 update (2026-07-19, later same day): user requested the solution be renamed to fit this
  project rather than keep its HR-domain name — `AI.HR.Api` (and all 7 sibling projects:
  `.BL`/`.DAL`/`.EF`/`.Models`/`.Repoistories`/`.Common`/`.Api.Tests`) renamed to
  `HC.AI.Identity.Api` (folder now `hc_apis/az/hc_core_apis/HC.AI.Identity.Api/`), matching the `HC.AI.*`
  namespace family already used by `HC.AI.MAPI`. Namespaces, `using`s, `.csproj`/`.sln` references,
  and JWT `Issuer`/`Audience` config values updated to match; the `Repoistories` typo was fixed to
  `Repositories` in the process. `AiHrDbContext` class name and the `AiHrDb` connection-string key
  were left unchanged (not part of the requested rename scope). Rebuilt clean, EF migration history
  still recognized against `AI_HealthCarePatient` (all 5 migrations intact), and a live smoke test
  (bad-password login + `/roles`) confirmed the renamed API works correctly post-rename. Database
  itself (`AI_HealthCarePatient`, and the untouched source `AI_HR`) was not renamed or altered.
- PB025 (Angular wiring): found `HC.AI.Identity.Api`'s CORS policy allows origin
  `http://localhost:4201`, but this project's Angular dev server runs on `4200` (established since
  US008) — not fixed (wiring went through the Angular dev-server proxy instead, which sidesteps
  CORS entirely), but worth Dev .NET correcting since a direct-from-browser call would fail today.
  Also: live verification created a real throwaway user
  (`claude.verification.test@example.com`, userId 4014) in `AI_HealthCarePatient.Users` — no
  delete-user endpoint exists to clean it up; flagged for Dev .NET/SQL. **Cleaned up 2026-07-19**
  (row deleted directly via SQL).
- PB026 note: `AI.HealthCare.Patient.API` has a much wider blast radius than `HC.AI.Identity.Api`
  did — at least 20 files reference it by name across `hc_agile/` docs (architecture, capability
  tracker, worklogs, TASK013), `hc_qa/api/ai_hc_api/` (an entire QA test project named after it),
  `.gitignore`, and design/reference docs — not just its own solution folder. Whoever picks this up
  should scope the doc/QA-project impact before starting, not just the C# rename mechanics.
- PB027 raised because the user's actual goal wasn't cost avoidance per se, but a client/leadership-
  facing showcase of progress — a live Kanban board with sprint burndown reads as "real agile
  process" in a demo in a way a markdown table doesn't. `hc_agile/` stays the source of truth
  (git-tracked, portfolio commit history); Azure Boards is purely the visual/demo layer mirroring
  the same PB items. User created the Azure DevOps project (**MVK AI Health Care**) themselves;
  assigned to Dev DevOps Agent (`TASK014`) to work out the mirroring/setup details directly with the
  user.
- PB027 update (2026-07-20): PAT-based REST API access to the Azure DevOps org confirmed working
  (stored in Windows Credential Manager per `hc_devops/PAT_SETUP_GUIDE.md`, not committed anywhere).
  Discovered the project's process template was initially **Basic** (Epic/Issue/Task only); user
  then changed it themselves and it now has native Feature/User Story types too. Before TASK014's
  actual scope (mirroring PB001-PB026) was located, one standalone Epic #1 "Azure DevOps
  Environment Setup" + child Issue #2 "Set up the environment" were created — these are generic
  process items, not part of the PB mirroring structure, and should be reconciled (kept as a
  meta-tracking item, or closed/deleted) once real mirroring work starts. See
  `hc_agile/worklogs/dev_devops/20260720_125630_azure_devops_environment_setup.md` for the full
  session log.
- PB027 update (2026-07-20, same day): started an Azure DevOps **Wiki** section (the project wiki
  `MVK-AI-Health-Care.wiki` already existed, auto-provisioned — confirmed the existing PAT scope
  also covers wiki read/write). Published `/aihcweb` → `/aihcweb/Architecture` →
  `/aihcweb/Architecture/Architecture-1`, the first numbered architecture snapshot for the Angular
  UI (structure review + two gaps found: inconsistent per-feature routing files, no HTTP
  interceptor attaching the JWT to outgoing API calls — documented, not fixed). Deliberately
  numbered/versioned rather than a single page, so future reviews add `Architecture-2` etc. instead
  of overwriting history. `hc_agile/` remains the source of truth; the wiki is a browsable mirror.
  See `hc_agile/worklogs/dev_devops/20260720_143000_wiki_aihcweb_architecture.md`.
- PB028/PB029/PB030 raised together (2026-07-20): same-day extension of the wiki architecture
  work to the two real backend APIs — `HC.AI.MAPI` and `HC.AI.Identity.Api` each got their own
  `/<Api>` → `/<Api>/Architecture` → `/<Api>/Architecture/Architecture-1` page tree, read
  directly from `Program.cs` DI wiring and controller source (not from stale worklog docs).
  Findings from that pass: `HC.AI.MAPI` has no authentication wired at all (pairs with PB028's
  routing/interceptor gap — a token would have nowhere to go even if Angular sent one);
  `HC.AI.MAPI.Guardrail` and `HealthcareQueryTool` exist but aren't in the live request path
  (matches PB020); `HC.AI.Identity.Api`'s real folder moved to
  `hc_apis/az/hc_core_apis/HC.AI.Identity.Api` (docs still said `hc_ai_in/mapi/...`, now
  corrected on the wiki page); `HC.AI.Identity.DAL` is an unused empty project. None of these
  were fixed, only documented. What Architecture-1 does NOT yet cover, and what PB028-030 exist
  to track: **sequence diagrams** (interaction-over-time between actors/components — e.g. Doctor
  → Angular → HC.AI.MAPI → Ollama → back) and a broader **technical design flow** write-up per
  service, both as text flow charts, kept updated as each system's real implementation changes
  rather than written once and left stale. See
  `hc_agile/worklogs/dev_devops/20260720_143000_wiki_aihcweb_architecture.md` for the
  established pattern (numbered snapshot pages, `hc_agile/` as source of truth, wiki as mirror)
  that PB028-030 continue.
- PB031 raised 2026-07-20: user's actual end goal is an **Admin persona that manages Doctor and
  Patient details** (a future admin console — record CRUD/oversight, not built yet, not scoped by
  this PB). What's needed first is the entry point: an Admin login/signup pair under `/admin/...`
  routes, structurally identical to the existing Doctor/Patient auth pages (`features/auth/`) but
  a separate module. No `Admin` role exists in `HC.AI.Identity.Api`'s `Roles` table (simplified
  to just `1 Doctor`/`2 Patient` per the 2026-07-19 migration) and no admin-management endpoints
  exist anywhere — signing up for real against the current backend with a fabricated RoleId would
  fail the FK constraint. Built mock-first (same pattern as the original US009 auth pages before
  `HC.AI.Identity.Api` existed) rather than blocking on backend work. Follow-up backlog item
  needed once real Admin-role + Doctor/Patient-management endpoints are wanted — not filed yet,
  since scope (what can an Admin actually do to a Doctor/Patient record?) hasn't been defined.
  Mirrored into Azure DevOps as Epic "Admin Management" > Feature "Admin Identity" > User Stories
  "Admin Login Page" / "Admin Sign-Up Page" — the first PB item to go through TASK014's intended
  Epic/Feature/User Story mirroring structure (Epic #1/Issue #2 from earlier were generic
  process items, not a PB mirror).
- PB031 update (2026-07-20, same day): built and verified. Drafted the two stories as US011/US012
  first, then found (via `git status`) that concurrent work outside this session had already
  claimed both numbers for the unrelated EPIC001/PB032 thread — renumbered to **US014**/US015
  before writing any code, same "investigate before assuming ownership" pattern as the earlier
  TASK009 collision. `ng build` clean, `ng test` 58/58 passing (up from 40). Azure DevOps items
  created and parent-links verified via `$expand=relations` (not just trusted from the create
  response — learned that lesson from the earlier wiki-content mistake this session): Epic **#44**
  "Admin Management" > Feature **#45** "Admin Identity" > User Story **#46** "Admin Login Page" /
  **#47** "Admin Sign-Up Page". See
  `hc_agile/worklogs/dev_angular/20260720_170000_admin_identity_mock.md`.
- PB032 raised 2026-07-20: user's plan for `HC.AI.MAPI` is that each persona gets two model
  roles - a Classification model (routes the request to a prompt-type) and one or more Executor
  models (answer once routed), each persona also carrying multiple prompts split the same way, all
  resolved from the DB by user -> `Roles.RoleId` rather than appsettings. Supersedes PB019 (which
  only covered "which single model for this persona") and formally tracks the prompt-dynamism note
  that previously had no backlog item at all. Key design call made during scoping: reuse the
  existing `Roles` table (PB023) as the persona-group concept instead of adding a duplicate - that
  table's own comment already anticipated this ("later we will change accordingly"). Several
  design questions intentionally left open for user sign-off rather than assumed - see
  `hc_agile/architecture/design_patterns/persona_dynamic_llm_prompt_schema.md`. Explicitly not
  built yet: schema is proposed only, EPIC001/US011-US013 are Draft, nothing created in any
  database or Azure DevOps.
- PB032 update (2026-07-20, same day): user asked to implement the resolution mechanism with mock
  models rather than wait on the schema sign-off — built in `HC.AI.MAPI` (`Models.Persona`,
  `BL.Persona.PersonaLlmConfigMockProvider`, `BL.Persona.PersonaModelResolutionBL`,
  `PersonaModelResolutionController`), Doctor-only, one placeholder prompt-type (`"General"`) since
  real prompt-type values are explicitly not being guessed at. `dotnet build` clean; live-verified
  both resolved and correctly-unresolved cases (unknown code, unseeded Patient persona both 404).
  **Not** wired into the live Doctor `provide-prompt` endpoint — doing so would silently answer the
  fallback-behavior questions US012/US013 flag as needing Product Owner sign-off. No DB tables, no
  real Ollama classification call — this is the config-resolution mechanism only. See TASK017 /
  `hc_agile/worklogs/dev_dotnet/20260720_180000_persona_resolution_mock.md`.
- PB034 raised by the user as its own high-priority item, distinct from (but overlapping) PB019/
  PB032: the resolution *mechanism* (`PersonaModelResolutionBL`, `LLMOptionsFactory`) already
  exists and takes a persona as input, but nothing yet **requires** every prompt request to set one
  or verifies every request path actually does — including the newly-added Admin persona
  (`HC.AI.Admin.API`, PB033), which has no prompt/LLM involvement at all yet. Not scoped in detail;
  flagged so it isn't lost, not yet assigned to an agent.
- PB034 update (2026-07-21): Dev Angular Agent found a real bug while reviewing the new
  `PromptRequest.Persona` field — `DoctorPromptMapper` (the **Doctor** endpoint's own mapper) had
  a hardcoded `Persona = APIConstants.PatientExecutorPersonaName`, tagging every Doctor request
  internally as Patient. By the time this was investigated, concurrent Semantic Kernel work had
  already fixed the mapper itself (computing a `PromptItem.ModelKey` correctly from
  `request.Persona`) and added an `HCPatientExecutor` appsettings section + pulled
  `hc-patient-executor:1.1` in Ollama — but `LLMModelBL` (the class that actually resolves LLM
  options) still ignored `ModelKey` and hardcoded the Doctor section regardless. Fixed in
  TASK019: `LLMModelBL` now resolves `model.PromptItem.ModelKey`. Live-verified directly against
  `HC.AI.MAPI` (bypassing the Angular proxy): `persona: "Doctor"` → `hc-doctor-executor:latest`,
  `persona: "Patient"` → `hc-patient-executor:1.1` — confirms the branch actually switches models.
  Angular's `doctor-chat.service.ts` also updated to send `persona: "Doctor"` explicitly (it
  previously sent no persona field at all). Still config-based (appsettings), not database-backed
  — PB019/PB032/EPIC001 remain open for that. See
  `hc_agile/worklogs/dev_semantic_kernel/20260721_180000_persona_model_selection_wired.md`.
- PB035 raised 2026-07-21: user clarified Admin is not a chat/prompt persona the way Doctor and
  Patient are — Admin doesn't converse with an LLM at all, so there's no "Admin executor model" to
  resolve and PB034's Admin-persona gap isn't actually a gap, just scope that doesn't apply.
  Explicitly deprioritized (Low) and deferred — user wants to discuss what, if anything, Admin
  needs from the LLM/prompt side later, and wants focus back on Doctor + Patient in the meantime.
  Nothing built or changed for this item; it exists so the earlier PB034 note doesn't read as an
  outstanding gap that needs closing.
- PB036 raised 2026-07-22: user explicitly authorized closing US010's "no Patient backend exists"
  gap (previously deferred with no PB of its own) after asking whether Patient had an executor
  pipeline like Doctor's. Built a full parallel `HC.AI.MAPI` pipeline
  (`PatientController -> PatientService -> PatientPromptMapper -> LLMModelBL ->
  PatientSemanticProcess -> PatientPromptProvider`), reusing the `HCPatientExecutor` config/model
  added under PB034. `PatientPromptMapper` deliberately hardcodes persona/`ModelKey` server-side
  rather than trusting a client-supplied value — the same lesson as the original
  `DoctorPromptMapper` bug (PB034/US021). Angular: `PatientChatMockService` removed,
  `PatientChatPage` now calls a real `PatientChatService`, `patient-chat-page.spec.ts` rewritten
  against `HttpTestingController`. Live-verified end-to-end through the actual Angular dev-server
  proxy (not just a direct API call): `modelUsed: "hc-patient-executor:1.1"`, real generated
  reply. Tracked as US023/TASK020; US010 updated to point here rather than restating "done" in
  place. Status: Done.
