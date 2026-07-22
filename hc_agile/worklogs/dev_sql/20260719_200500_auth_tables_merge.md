# Dev SQL Agent — Work Log
## Date: 2026-07-19
## Subject: Add Users/Roles/OcrDocuments schema to AI_HealthCarePatient (PB023)

## Context
Paired with Dev .NET Agent to consolidate the previously-separate `AI_HR` LocalDB database
(used only by the newly-discovered `AI.HR.Api` auth solution) into `AI_HealthCarePatient`, the
project's existing primary database. See the companion
[Dev .NET worklog](../dev_dotnet/20260719_200000_auth_db_merge_ai_hr_api.md) for the full context
and the code/config side of this change.

## What Was Done
- Verified no table-name collisions before merging: `AI_HealthCarePatient` had 23 existing tables
  (Patients, Encounters, Conditions, ..., plus PB013's `TableWhitelist`/`ColumnWhitelist`/
  `JoinWhitelist`/`QueryAuditLog`); `AI_HR`'s schema only adds `Users`, `Roles`, `OcrDocuments` —
  clean, no conflicts
- Schema was applied via Dev .NET's `dotnet ef database update` (EF Core migrations), not raw SQL
  — but documented it as SQL DDL anyway for consistency with how this folder tracks every other
  table in the project: `tables/002_auth_tables_users_roles_ocrdocuments.sql`
- Updated `README.md` with a new "Authentication tables (PB023)" section explaining the merge and
  pointing at the reference script
- Confirmed post-merge: `AI_HealthCarePatient` now has 26 tables total; existing data untouched
  (`Patients` still had its 113 rows); `Roles` seeded with 7 rows (HR-domain names, carried as-is
  from `AI_HR` — see open item below); `Users`/`OcrDocuments` populated with the 9 + 2 rows copied
  from `AI_HR`

## Decisions Made
- Documented the new tables as a numbered script (`002_...sql`) following the same convention as
  `001_whitelist_and_audit_tables.sql`, even though the actual DDL execution path was EF
  migrations rather than running this script directly — keeps `hc_data_source/hc_sql` as the
  single place to look up "what tables exist and why," per this folder's ownership
- Did not rename or restructure anything from the source schema (column names, types, `RoleId`
  seed values) — straight carry-over, no schema redesign attempted in this pass

## Pending / Next Steps
- `Roles.RoleName` values (HR Manager, HR Executive, Recruiter, Payroll Manager, Team Lead,
  Developer, Other) don't match this project's Doctor/Patient persona model — flagged to Product
  Owner/Architect via `TASK013`, not resolved here
- No new stored procedures or whitelist entries were added for the auth tables — they're accessed
  directly through `AI.HR.Api`'s EF layer, not through the `usp_ExecuteHealthcareQuery` guardrail
  path (that path is specific to the LLM-driven Doctor Chat query DSL from PB013, not user auth)
- Source `AI_HR` database left running/untouched, per instruction — no cleanup task raised for it

## References
- [hc_data_source/hc_sql/README.md](../../../hc_data_source/hc_sql/README.md)
- [tables/002_auth_tables_users_roles_ocrdocuments.sql](../../../hc_data_source/hc_sql/tables/002_auth_tables_users_roles_ocrdocuments.sql)
- [BACKLOG.md](../../product_owner/backlog/BACKLOG.md) — PB023
