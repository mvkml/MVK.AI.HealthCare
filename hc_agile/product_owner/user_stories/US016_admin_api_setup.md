# US016 - Set Up HC.AI.Admin.API (ASP.NET Core REST API scaffold)

**As an** Admin
**I want to** have a dedicated backend REST API for admin functionality
**So that** the Admin persona (currently mock-only login/signup, US014/US015) has a real API to
be wired to once its endpoints are defined

## Background
Part of the broader Admin epic (Epic #44 "Admin Management" in Azure DevOps, alongside Feature
#45 "Admin Identity" / User Stories #46-#47 for the mock UI). US014 explicitly flagged that no
`Admin` role or admin-specific endpoints exist anywhere yet — this story creates the API project
itself, as a separate solution (`HC.AI.Admin.API`) rather than extending `HC.AI.Identity.Api`,
matching this project's existing pattern of one dedicated API per domain (`HC.AI.MAPI`,
`HC.AI.Identity.Api`, `AI.HealthCare.Patient.API`).

## Scope for this story
- Scaffold a new ASP.NET Core REST API solution named `HC.AI.Admin.API`, layered consistently
  with the project's existing `.NET` APIs (Api / BL / DAL-or-Repositories / EF / Models / Common)
- Location: `hc_apis/az/hc_core_apis/HC.AI.Admin.API/` (matches where `HC.AI.Identity.Api` and
  `AI.HealthCare.Patient.API` live)
- **Endpoint details are explicitly not yet provided by the user** — this story covers project
  setup/scaffold only (solution structure, `Program.cs`, DB context wiring convention, Swagger).
  Do not invent endpoints speculatively.

## Explicitly out of scope for this story
- Actual admin endpoints (CRUD for Doctor/Patient management, or whatever the user specifies) —
  blocked on the user providing endpoint details
- Wiring the existing mock Admin Angular UI (US014/US015) to this API — separate future story
- An `Admin` role/table decision (new dedicated DB vs. extending `AI_HealthCarePatient`'s `Roles`
  table from `HC.AI.Identity.Api`) — not decided yet, flag to Architect when endpoint scope is known

## Priority: High
## Status: Done — superseded in scope by [US020](US020_admin_signup_login.md), which delivered the
scaffold plus the concrete signup/login endpoints once the user provided them
## Sprint: Unscheduled
## ADO mirror: Epic #44 "Admin Management" > Feature #45 "Admin Identity" > [User Story #48](https://dev.azure.com/mvishnukiran05/a9d525a0-0840-4b29-8475-fbb3d23acb9d/_workitems/edit/48)
(ADO state not yet updated — user asked to hold local-only sync for the follow-on work; see US020)
