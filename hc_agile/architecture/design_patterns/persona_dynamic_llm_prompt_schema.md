# Persona-Driven Dynamic LLM + Prompt Resolution — Proposed Schema

**Status:** Draft — proposed only, not created in any database. For review.
**Raised by:** user, 2026-07-20, discussed with Architect/Product Owner/Scrum Master.
**Related:** PB019 (DB-backed LLM/model selection — this supersedes/expands it), Prompt Layer scope
note (dynamic/DB-backed prompts), PB023 (existing `Roles`/`Users` schema).

## What this solves

Today, `LLMOptionsFactory` resolves a single LLM configuration per persona from static
`appsettings.json`, and each persona's prompt is a single hardcoded provider. The goal: make both
of those dynamic and DB-driven, and support **two model roles per persona**:

- **Classification model** — reads the incoming request and decides which "type" of request this
  is (routes to the right context/agent), rather than answering it directly.
- **Executor model** — the model (and prompt) that actually answers, once the classification model
  has identified the type. A persona can have *multiple* executor models/prompts — one per
  request-type/agent — while typically having a single classification (router) model.

## Key design decision — reuse `Roles`, don't duplicate it

`hc_data_source/hc_sql/tables/002_auth_tables_users_roles_ocrdocuments.sql` already has:
- `Roles` (`RoleId`, `RoleName`) — currently seeded `1 = Doctor`, `2 = Patient`
- `Users.RoleId` FK — every user already belongs to exactly one role

That table's own comment says this was left "a starting point... later we will change accordingly."
**This proposal treats `Roles` as the persona-group table already** — new tables below FK to
`Roles.RoleId` directly. No new `PersonaGroup`/`PersonaType` table is introduced, since one already
exists and would otherwise duplicate it.

**Open question for you to confirm:** does a user ever need to belong to *more than one* persona at
once (e.g. someone who is both a Doctor and an Admin)? Today `Users.RoleId` is a single FK — that
already gives us the "automatically falls into that persona's ecosystem" behavior you described. If
one-role-per-user is still correct, no new mapping table is needed at all. If a user can carry
multiple personas simultaneously, we'd need a `UserRoleMapping` (many-to-many) table instead of the
existing single FK — **flagging this rather than assuming**, since it changes the shape.

## Proposed new tables

### `PersonaPromptType`
The "type of request" a classification model routes to, scoped per persona/role — this is what the
classification model's output value should match.

| Column | Type | Notes |
|---|---|---|
| `PersonaPromptTypeId` | `INT IDENTITY PK` | |
| `RoleId` | `INT FK -> Roles.RoleId` | which persona this type belongs to |
| `Code` | `NVARCHAR(50)` | machine-readable key the classification model outputs, e.g. `SymptomLookup` |
| `Name` | `NVARCHAR(100)` | human-readable label |
| `IsActive` | `BIT` | |
| `CreatedDate` | `DATETIME2` | |
| `UpdatedDate` | `DATETIME2 NULL` | |

*Open question: what are the actual prompt-type values for Doctor/Patient? Not guessing at these —
needs your input before this table can be seeded.*

### `PersonaLLMOption`
Multiple LLM options per persona, split by model role.

| Column | Type | Notes |
|---|---|---|
| `PersonaLLMOptionId` | `INT IDENTITY PK` | |
| `RoleId` | `INT FK -> Roles.RoleId` | |
| `ModelRole` | `NVARCHAR(20)` | `'Classification'` or `'Executor'` |
| `PersonaPromptTypeId` | `INT FK -> PersonaPromptType, NULL` | NULL for Classification (single router model); set for Executor (one row per request-type/agent) |
| `ModelName` | `NVARCHAR(100)` | e.g. `qwen2.5:7b` |
| `Provider` | `NVARCHAR(50)` | e.g. `Ollama`, `OpenAI` |
| `IsDefault` | `BIT` | which option is picked when multiple are active |
| `Priority` | `INT` | fallback order, if that's wanted |
| `IsActive` | `BIT` | |
| `CreatedDate` | `DATETIME2` | |
| `UpdatedDate` | `DATETIME2 NULL` | |

*Open question: is "multiple options per persona" a fallback chain (try next on failure) or just
an admin-configurable single active choice at a time? Affects whether `Priority` does real work or
`IsDefault` alone is enough.*

### `PersonaPrompt`
The actual prompt content, same role/type split as `PersonaLLMOption`.

| Column | Type | Notes |
|---|---|---|
| `PersonaPromptId` | `INT IDENTITY PK` | |
| `RoleId` | `INT FK -> Roles.RoleId` | |
| `ModelRole` | `NVARCHAR(20)` | `'Classification'` or `'Executor'` |
| `PersonaPromptTypeId` | `INT FK -> PersonaPromptType, NULL` | NULL for the classification/router prompt; set for Executor prompts |
| `PromptText` | `NVARCHAR(MAX)` | |
| `Version` | `INT` | |
| `IsActive` | `BIT` | |
| `CreatedDate` | `DATETIME2` | |
| `UpdatedDate` | `DATETIME2 NULL` | |

## Flow this enables (conceptual, not code-level)

1. Request arrives for a user → resolve `RoleId` from `Users` (existing).
2. Look up that role's active Classification model + prompt from `PersonaLLMOption`/`PersonaPrompt`
   (`ModelRole = 'Classification'`, `PersonaPromptTypeId IS NULL`).
3. Classification model returns a `PersonaPromptType.Code`.
4. Look up the matching Executor model + prompt for that role + prompt-type
   (`ModelRole = 'Executor'`, `PersonaPromptTypeId = <resolved type>`).
5. Executor model runs with its resolved prompt, returns the response.

## Still open before this can become real DDL

1. Single-role-per-user confirmed, or need many-to-many `UserRoleMapping`?
2. Actual `PersonaPromptType` values per persona (Doctor's types, Patient's types) — not invented
   here.
3. Fallback semantics for multiple active `PersonaLLMOption` rows (`Priority` real vs. cosmetic).
4. Naming: keep `Roles`/`RoleId` as-is, or rename conceptually now that it's carrying more meaning
   than "auth role"? (Pure naming question — no behavior change either way.)
