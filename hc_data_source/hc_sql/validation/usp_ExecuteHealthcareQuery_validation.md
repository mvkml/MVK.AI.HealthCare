# Validation — `dbo.usp_ExecuteHealthcareQuery`

Run against the real `AI_HealthCarePatient` LocalDB (`(localdb)\MSSQLLocalDB`), 2026-07-17.
Script: `hc_data_source/hc_sql/validation/validate_usp.sql` (companion, run via `sqlcmd -i`).

Deploy order used: `tables/001_whitelist_and_audit_tables.sql` → `seed/001_doctor_persona_whitelist_seed.sql` → `procedures/usp_ExecuteHealthcareQuery.sql`.

## Test 1 — single-patient lookup, exact match (`eq`)

```sql
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor', @RootTable = 'Patients',
    @FiltersJson = '[{"column":"Last","op":"eq","value":"Altenwerth646"}]',
    @MaxRecords = 10;
```
**Result:** 1 row — `Marquita692 Altenwerth646`, Boston, MA. ✅ Matches the DB's only patient with that last name.

## Test 2 — partial match (`contains`)

```sql
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor', @RootTable = 'Patients',
    @FiltersJson = '[{"column":"Last","op":"contains","value":"Alten"}]',
    @MaxRecords = 5;
```
**Result:** Same 1 row. ✅ `LIKE '%Alten%'` behaves correctly; confirms the value was bound as a parameter (no injection surface even though it's inside a `LIKE`).

## Test 3 — dynamic join (`Encounters` → `Patients`), explicit select + order

```sql
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor', @RootTable = 'Encounters', @JoinTable = 'Patients',
    @SelectColumnsCsv = 'Id,Start,Description,ReasonDescription',
    @OrderByColumn = 'Start', @OrderByDirection = 'DESC';
```
**Result:** 10 rows (default `@MaxRecords`), correctly ordered newest-`Start`-first, only the 4 requested columns projected. ✅ Proves the join-whitelist path end-to-end.

## Test 4 — MaxRecords ceiling enforcement

```sql
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor', @RootTable = 'Encounters', @MaxRecords = 100;
```
**Result:** 50 rows returned, not 100. Audit log confirms `MaxRecordsRequested = 100`, `MaxRecordsApplied = 50`. ✅ Hard ceiling holds regardless of what's requested.

## Test 5 — root table not on the whitelist

```sql
EXEC dbo.usp_ExecuteHealthcareQuery @Persona = 'Doctor', @RootTable = 'Claims', @MaxRecords = 10;
```
**Result:** `Caught expected error: Table "Claims" is not an allow-listed root table for persona "Doctor".` ✅ Rejected before touching data — `Claims`/billing is correctly out of Doctor-persona scope per the locked design doc.

## Test 6 — join not on the whitelist

```sql
EXEC dbo.usp_ExecuteHealthcareQuery @Persona = 'Doctor', @RootTable = 'Patients', @JoinTable = 'Claims', @MaxRecords = 10;
```
**Result:** `Caught expected error: Join Patients -> Claims is not allow-listed for persona "Doctor".` ✅ An LLM can't invent an arbitrary join path — only pairs explicitly seeded in `JoinWhitelist` work.

## Test 7 — `@ProviderId` scoping ("recent patients" for *this* doctor)

```sql
EXEC dbo.usp_ExecuteHealthcareQuery
    @Persona = 'Doctor', @RootTable = 'Encounters',
    @SelectColumnsCsv = 'Id,PatientId,ProviderId,Description',
    @ProviderId = 'F19A10F9-21C3-3526-8577-0032061084A0', @MaxRecords = 5;
```
**Result:** 5 rows, every one with `ProviderId = F19A10F9-...`. ✅ Confirms the scoping filter is applied even though `Encounters` was queried without a join to `Providers`.

## Test 8 — audit trail

```sql
SELECT Persona, RootTable, JoinTable, MaxRecordsRequested, MaxRecordsApplied, RowsReturned, ExecutedAtUtc
FROM dbo.QueryAuditLog ORDER BY Id;
```
**Result:** 5 rows logged — one per *successful* execution (Tests 1–4, 7). ✅ as far as it goes, but see the gap noted below.

## Gap found during validation (not yet a backlog item)

Tests 5 and 6 (rejected table/join) do **not** produce an audit row — the procedure `RETURN`s before reaching the `INSERT INTO QueryAuditLog`. For a healthcare system, a rejected attempt to touch `Claims` is arguably *more* worth auditing than a successful one (it's a guardrail-bypass attempt, whether from a bug or a malicious prompt). Recommend logging rejections too, with a `Rejected` outcome column. I'd add this as **PB018** unless you'd rather fold it into PB017 (hardening).

## Open question carried over

Test 7 shows `@ProviderId` scoping works mechanically, but PB016 (should "recent patients" *default* to hospital-wide or per-provider?) is still unresolved — this test only proves the plumbing, not the product default.
