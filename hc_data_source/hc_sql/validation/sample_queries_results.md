# Sample queries — `dbo.usp_ExecuteHealthcareQuery`

Additional scenarios beyond the core acceptance tests in `usp_ExecuteHealthcareQuery_validation.md`.
Companion script: `sample_queries.sql`. Run against `AI_HealthCarePatient` LocalDB, 2026-07-17/18.

## Sample 1 — multiple filters combined (AND)

```sql
@FiltersJson = '[{"column":"Gender","op":"eq","value":"F"},{"column":"State","op":"eq","value":"Massachusetts"}]'
```
**Result:** 5 rows, all female patients in Massachusetts. ✅ Multiple filter slots combine with `AND` correctly.

## Sample 2 — `gt` filter on a date column

```sql
@RootTable = 'Encounters', @FiltersJson = '[{"column":"Start","op":"gt","value":"2026-01-01"}]', @OrderByColumn = 'Start', @OrderByDirection = 'ASC'
```
**Result:** 5 encounters, earliest first, all with `Start > 2026-01-01`. ✅ Date comparison and ascending sort both work.

## Sample 3 — no filters, defaults only

```sql
@Persona = 'Doctor', @RootTable = 'Patients'
```
**Result:** 10 rows (default `@MaxRecords`), all whitelisted `Patients` columns. ✅ Confirms the "no filter object at all" path the LLM will hit for open-ended questions like "list some patients."

## Sample 4 — filter matches nothing

```sql
@FiltersJson = '[{"column":"Last","op":"eq","value":"NoSuchPatientXYZ"}]'
```
**Result:** 0 rows, no error. ✅ Important for the Chat prompt — "no results" must be a clean empty set the LLM can report honestly, not an exception.

## Sample 5 — `Encounters` joined to `Conditions` (found a gap, then fixed)

First attempt used `@RootTable='Conditions'` directly — correctly **rejected** (`Conditions` isn't seeded as a root table). Second attempt exposed a bigger issue: **no join path reached `Conditions` at all**, from any root — the seed data made it an orphaned table. Added the missing whitelist row (`Encounters → Conditions`, joined the reverse way: `Encounters.Id = Conditions.EncounterId`, since the FK lives on `Conditions`, not `Encounters`) and re-ran:

```sql
@RootTable = 'Encounters', @JoinTable = 'Conditions', @SelectColumnsCsv = 'Id,Description'
```
**Result:** 5 rows — encounter IDs with their descriptions, correctly joined to `Conditions`. ✅ after the fix. **This is now reflected in `seed/001_doctor_persona_whitelist_seed.sql`** — redeploy that file if you're setting this up on another environment.

## Sample 6 — `@MaxRecords = 0`

**Result:** 10 rows (falls back to default), not 0. ✅ `0` is treated as "not a valid ceiling," same as `NULL` or negative — matches the procedure's clamp logic (`< 1` → default 10).

## Sample 7 — unrecognized persona

```sql
@Persona = 'Nurse', @RootTable = 'Patients'
```
**Result:** `Caught expected error: Table "Patients" is not an allow-listed root table for persona "Nurse".` ✅ Persona scoping works — only `'Doctor'` rows exist in `TableWhitelist` today, so any other persona is rejected outright, not silently given Doctor-level access.

## Sample 8 — requesting a non-whitelisted column (`Ssn`)

```sql
@SelectColumnsCsv = 'First,Last,Ssn'
```
**Result:** only `First, Last` returned — `Ssn` silently dropped. ✅ This is the guardrail proof that matters most for a healthcare system: even if the LLM's generated query shape asks for a sensitive column, it never reaches the result set unless someone explicitly adds it to `ColumnWhitelist`.

## Net finding worth flagging

Sample 5's first pass caught a real seeding gap (an unreachable table) before it reached the Doctor Chat prompt — exactly the kind of thing that's cheap to catch here and expensive to catch after the LLM is wired up. Worth doing the same reachability check (every `TableWhitelist` row has at least one path in from a root) whenever a new persona or table is added — could be a simple query against the two tables rather than a manual review each time; happy to write that as a one-off check if useful.
