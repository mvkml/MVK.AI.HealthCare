# Architecture — Per-Module Text-Flow

## Owned by
Architect Agent

## Purpose
Every one of the 18 domains (`Patients`, `Organizations`, `Providers`, `Payers`, `PayerTransitions`, `Encounters`, `Conditions`, `Allergies`, `Medications`, `Careplans`, `Procedures`, `Immunizations`, `Observations`, `Devices`, `Supplies`, `ImagingStudies`, `Claims`, `ClaimTransactions`) is built to the **same architecture**, just with different fields/FKs. This doc is the one generic text-flow diagram that applies to all of them — a module-specific version is just this diagram with `<Domain>` substituted.

---

## Request flow (write path — e.g. `POST /api/<domain>`)

```
HTTP Request (JSON body)
      |
      v
<Domain>Request (DTO, no Id)                         [AI.HealthCare.Patient.Models/<Domain>/]
      |
      v
<Domain>sController.Create()                          [AI.HealthCare.Patient.API/Controllers/]
      |
      v
I<Domain>ValidationService.Validate()                  [AI.HealthCare.Patient.BL/<Domain>/]
      |          \
      | valid      \ invalid
      v             v
I<Domain>BL.Create()      400 BadRequest(<Domain>Response { IsNotValid, Message })
      |
      v
ToItem(request) -> <Domain>Item                        [private mapping inside BL]
      |
      v
I<Domain>Repository.Create(item)                       [AI.HealthCare.Patient.Repositories/<Domain>/]
      |
      v
I<Domain>Mapper.ToEntity(item) -> Ef<Domain>            [AI.HealthCare.Patient.Repositories/<Domain>/]
      |
      v
PatientDbContext.<Domain>s.Add(entity) + SaveChangesAsync()   [AI.HealthCare.Patient.EF/DBContexts/]
      |
      v
SQL Server table (DeleteBehavior.Restrict on all FKs)
      |
      v
I<Domain>Mapper.ToModel(entity) -> <Domain>Item  (round trip back up)
      |
      v
BL.ToResponse(item) -> <Domain>Response
      |
      v
200 OK(<Domain>Response)
```

## Read flow (`GET /api/<domain>/{id}`)

```
HTTP Request (Id in route)
      |
      v
<Domain>sController.GetById(id)
      |
      v
I<Domain>BL.GetById()
      |
      v
I<Domain>Repository.GetById(id)  -->  PatientDbContext  -->  SQL Server
      |
      v
I<Domain>Mapper.ToModel(entity) -> <Domain>Item
      |          \
      | found       \ not found
      v               v
BL.ToResponse(item)     404 NotFound("<Domain> not found.")
      |
      v
200 OK(<Domain>Response)
```

## GetAll flow (`GET /api/<domain>`)

```
Controller.GetAll() -> BL.GetAll() -> Repository.GetAll() -> Mapper.ToModel() per row -> List<<Domain>Item>
      |
      v
200 OK(List<<Domain>Item>)   <-- NOTE: returns raw Items, not Responses (no PII masking on list endpoints — tracked in backlog)
```

## Layer responsibility summary

| Layer | Project | Knows about |
|---|---|---|
| Controller | `AI.HealthCare.Patient.API` | HTTP only — routes, status codes, `[FromBody]`/`[FromQuery]` binding |
| ValidationService | `AI.HealthCare.Patient.BL` | Request-shape rules only (required fields, non-empty FKs) — no DB existence checks |
| BL | `AI.HealthCare.Patient.BL` | Business flow + Request/Item/Response DTO mapping |
| Repository | `AI.HealthCare.Patient.Repositories` | CRUD against `PatientDbContext` only |
| Mapper | `AI.HealthCare.Patient.Repositories` | Entity <-> Item translation only (SRP-extracted from Repository) |
| EF Entity/DbContext | `AI.HealthCare.Patient.EF` | Table shape, FK relationships, `DeleteBehavior.Restrict` |
| Models (DTOs) | `AI.HealthCare.Patient.Models` | Shape only, no behavior — `Request`/`Item`/`Response`/`<Domain>sModel` envelope |

## Status per module (which parts of the flow are live end-to-end)

| # | Module | Mapper | Repository | BL | Controller | Full flow live? |
|---|---|---|---|---|---|---|
| 1 | Patients | Yes | Yes | Yes | Yes | Yes (+ `includePii` masking branch) |
| 2 | Organizations | Yes | Yes | Yes | Yes | Yes |
| 3 | Providers | Yes | Yes | Yes | Yes | Yes |
| 4 | Payers | Yes | Yes | Yes | Yes | Yes |
| 5 | PayerTransitions | Yes | Yes | Yes | Yes | Yes (Id is `long`, not `Guid`) |
| 6 | Encounters | Yes | Yes | Yes | Yes | Yes (+ `GET /by-patient/{patientId}` scoped query) |
| 7 | Conditions | Yes | Yes | Yes | Yes | Yes (Id is `long`; + `GET /by-patient/{patientId}` scoped query) |
| 8 | Allergies | Yes | Yes | Yes | Yes | Yes (Id is `long`; + `GET /by-patient/{patientId}` scoped query) |
| 9 | Medications | Yes | Yes | Yes | Yes | Yes (Id is `long`; + `GET /by-patient/{patientId}` scoped query) |
| 10 | Careplans | Yes | Yes | Yes | Yes | Yes (+ `GET /by-patient/{patientId}` scoped query) |
| 11 | Procedures | Yes | Yes | Yes | Yes | Yes (Id is `long`; + `GET /by-patient/{patientId}` scoped query) |
| 12 | Immunizations | Yes | Yes | Yes | Yes | Yes (Id is `long`, `Date` required non-nullable; + `GET /by-patient/{patientId}` scoped query) |
| 13 | Observations | Yes | Yes | Yes | Yes | Yes (Id is `long`, `Date` required non-nullable; + `GET /by-patient/{patientId}` scoped query) |
| 14 | Devices | Yes | Yes | Yes | Yes | Yes (Id is `long`; + `GET /by-patient/{patientId}` scoped query) |
| 15 | Supplies | Yes | Yes | Yes | Yes | Yes (Id is `long`, `Date` required non-nullable; + `GET /by-patient/{patientId}` scoped query) |
| 16 | ImagingStudies | Yes | Yes | Yes | Yes | Yes (`Date` required non-nullable; + `GET /by-patient/{patientId}` scoped query) |
| 17 | Claims | Yes | Yes | Yes | Yes | Yes (large FK surface: `PrimaryPatientInsuranceId`/`SecondaryPatientInsuranceId` → Payers, `ReferringProviderId`/`SupervisingProviderId` → Providers, `AppointmentId` → Encounters; + `GET /by-patient/{patientId}` scoped query) |
| 18 | ClaimTransactions | Yes | Yes | Yes | Yes | Yes (`PlaceOfServiceId` → Organizations, `AppointmentId` → Encounters; + `GET /by-claim/{claimId}` scoped query instead of by-patient, since this domain is naturally scoped to a Claim) |

**All 18 domains complete.**

---
*Defined by: Architect Agent | Date: 2026-07-15*
