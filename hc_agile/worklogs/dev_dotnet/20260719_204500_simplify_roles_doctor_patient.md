# Dev .NET Agent — Work Log
## Date: 2026-07-19
## Subject: Simplify AI.HR.Api role model to Doctor/Patient (PB023 follow-up)

## Context
Immediately after the `AI_HR` → `AI_HealthCarePatient` DB merge (see
[20260719_200000_auth_db_merge_ai_hr_api.md](20260719_200000_auth_db_merge_ai_hr_api.md)), user
gave direct instruction: replace the inherited HR-domain role model (HR Manager, HR Executive,
Recruiter, Payroll Manager, Team Lead, Developer, Other) with just two roles — Doctor and Patient
— explicitly framed as a starting point ("later we will change accordingly").

## What Was Done
- Reassigned the 8 existing `AI_HealthCarePatient.Users` rows referencing roles 3-7 to `RoleId=2`
  (soon-to-be "Patient") **before** running the migration — required, since the migration's
  `DELETE FROM Roles WHERE RoleId IN (3,4,5,6,7)` would otherwise violate the `Users.RoleId` FK
- `AiHrDbContext.cs` — `Role.HasData` seed reduced from 7 HR roles to `{1, "Doctor"}`,
  `{2, "Patient"}`
- `UserBL.cs` — `DefaultSignUpRoleId` changed from `7` ("Other") to `2` ("Patient"); doc comment
  updated
- `UserRequest.cs` — doc comment updated to match
- Generated migration `SimplifyRolesToDoctorPatient` via `dotnet ef migrations add` — EF correctly
  scaffolded 5 `DeleteData` (roles 3-7) + 2 `UpdateData` (rename 1→Doctor, 2→Patient); flagged by
  EF as potentially data-lossy, which was expected and already handled by the reassignment step
  above
- Applied via `dotnet ef database update` — succeeded, no FK violations
- Verified: `Roles` now has exactly 2 rows (Doctor, Patient); all 9 `Users` rows correctly join to
  one of the two; `dotnet build AI.HR.Api.sln` still succeeds

## Decisions Made
- Default mapping for the 8 reassigned users: **Patient** (the lower-privilege, self-service-like
  default, mirroring the old default-to-"Other" pattern). Not asked account-by-account — these are
  mostly test/demo accounts (`Test User`, `Interface Test`, `Login Test`, `Test FP/UA/DI`), plus 2
  that seed data suggests belong to the user themselves. **Flagged to the user**: if any specific
  existing account should be Doctor instead of Patient, that's a one-row `UPDATE Users SET
  RoleId=1 WHERE Email='...'` — not done speculatively here
- The 1 user that already had `RoleId=1` before this change is now "Doctor" by inheritance of that
  ID, not a deliberate persona decision — same caveat applies

## Pending / Next Steps
- User said this two-role model is a starting point, not final — no further role/persona work
  planned until the user gives more detail
- `TASK013` updated to reflect this item as resolved (was previously an open blocker for Dev
  Angular Agent's signup persona-selection UI)

## References
- [BACKLOG.md](../../product_owner/backlog/BACKLOG.md) — PB023
- [TASK013](../../scrum/tasks/TASK013_US009_wire_real_auth_backend.md)
- [tables/002_auth_tables_users_roles_ocrdocuments.sql](../../../hc_data_source/hc_sql/tables/002_auth_tables_users_roles_ocrdocuments.sql)
