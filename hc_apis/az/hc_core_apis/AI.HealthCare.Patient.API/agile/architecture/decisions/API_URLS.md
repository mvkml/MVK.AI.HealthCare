# API URLs — AI.HealthCare.Patient.API

Tracks live endpoints as domains get their Controller built. Update whenever a new Controller/endpoint is added.

## Swagger / OpenAPI
- UI: `/swagger/index.html` (Development environment only)
- Spec: `/swagger/v1/swagger.json`

## Patients (`PatientsController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/patients?includePii={bool}` | Create a patient |
| GET | `/api/patients` | List all patients |
| GET | `/api/patients/{id}?includePii={bool}` | Get a patient by Id |
| PUT | `/api/patients/{id}?includePii={bool}` | Update a patient |
| DELETE | `/api/patients/{id}` | Delete a patient |

**`includePii` flag** (default `false`): when `true`, the response includes `Ssn`/`Drivers`/`Passport`, masked to the last 4 characters (e.g. `999-83-4938` → `***-**-4938`). When omitted/`false`, these fields are `null`. Demo/learning-purpose feature — not a substitute for real PII encryption at rest.

## Organizations (`OrganizationsController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/organizations` | Create an organization |
| GET | `/api/organizations` | List all organizations |
| GET | `/api/organizations/{id}` | Get an organization by Id |
| PUT | `/api/organizations/{id}` | Update an organization |
| DELETE | `/api/organizations/{id}` | Delete an organization |

## Providers (`ProvidersController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/providers` | Create a provider |
| GET | `/api/providers` | List all providers |
| GET | `/api/providers/{id}` | Get a provider by Id |
| PUT | `/api/providers/{id}` | Update a provider |
| DELETE | `/api/providers/{id}` | Delete a provider |

## Payers (`PayersController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/payers` | Create a payer |
| GET | `/api/payers` | List all payers |
| GET | `/api/payers/{id}` | Get a payer by Id |
| PUT | `/api/payers/{id}` | Update a payer |
| DELETE | `/api/payers/{id}` | Delete a payer |

## PayerTransitions (`PayerTransitionsController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/payertransitions` | Create a payer transition |
| GET | `/api/payertransitions` | List all payer transitions |
| GET | `/api/payertransitions/{id}` | Get a payer transition by Id (`long`) |
| PUT | `/api/payertransitions/{id}` | Update a payer transition |
| DELETE | `/api/payertransitions/{id}` | Delete a payer transition |

## Encounters (`EncountersController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/encounters` | Create an encounter |
| GET | `/api/encounters` | List all encounters |
| GET | `/api/encounters/by-patient/{patientId}` | List all encounters for a given patient (agent-friendly scoped query) |
| GET | `/api/encounters/{id}` | Get an encounter by Id |
| PUT | `/api/encounters/{id}` | Update an encounter |
| DELETE | `/api/encounters/{id}` | Delete an encounter |

## Conditions (`ConditionsController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/conditions` | Create a condition |
| GET | `/api/conditions` | List all conditions |
| GET | `/api/conditions/by-patient/{patientId}` | List all conditions for a given patient |
| GET | `/api/conditions/{id}` | Get a condition by Id (`long`) |
| PUT | `/api/conditions/{id}` | Update a condition |
| DELETE | `/api/conditions/{id}` | Delete a condition |

## Allergies (`AllergiesController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/allergies` | Create an allergy |
| GET | `/api/allergies` | List all allergies |
| GET | `/api/allergies/by-patient/{patientId}` | List all allergies for a given patient |
| GET | `/api/allergies/{id}` | Get an allergy by Id (`long`) |
| PUT | `/api/allergies/{id}` | Update an allergy |
| DELETE | `/api/allergies/{id}` | Delete an allergy |

## Medications (`MedicationsController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/medications` | Create a medication |
| GET | `/api/medications` | List all medications |
| GET | `/api/medications/by-patient/{patientId}` | List all medications for a given patient |
| GET | `/api/medications/{id}` | Get a medication by Id (`long`) |
| PUT | `/api/medications/{id}` | Update a medication |
| DELETE | `/api/medications/{id}` | Delete a medication |

## Careplans (`CareplansController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/careplans` | Create a careplan |
| GET | `/api/careplans` | List all careplans |
| GET | `/api/careplans/by-patient/{patientId}` | List all careplans for a given patient |
| GET | `/api/careplans/{id}` | Get a careplan by Id |
| PUT | `/api/careplans/{id}` | Update a careplan |
| DELETE | `/api/careplans/{id}` | Delete a careplan |

## Procedures (`ProceduresController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/procedures` | Create a procedure |
| GET | `/api/procedures` | List all procedures |
| GET | `/api/procedures/by-patient/{patientId}` | List all procedures for a given patient |
| GET | `/api/procedures/{id}` | Get a procedure by Id (`long`) |
| PUT | `/api/procedures/{id}` | Update a procedure |
| DELETE | `/api/procedures/{id}` | Delete a procedure |

## Immunizations (`ImmunizationsController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/immunizations` | Create an immunization |
| GET | `/api/immunizations` | List all immunizations |
| GET | `/api/immunizations/by-patient/{patientId}` | List all immunizations for a given patient |
| GET | `/api/immunizations/{id}` | Get an immunization by Id (`long`) |
| PUT | `/api/immunizations/{id}` | Update an immunization |
| DELETE | `/api/immunizations/{id}` | Delete an immunization |

## Observations (`ObservationsController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/observations` | Create an observation |
| GET | `/api/observations` | List all observations |
| GET | `/api/observations/by-patient/{patientId}` | List all observations for a given patient |
| GET | `/api/observations/{id}` | Get an observation by Id (`long`) |
| PUT | `/api/observations/{id}` | Update an observation |
| DELETE | `/api/observations/{id}` | Delete an observation |

## Devices (`DevicesController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/devices` | Create a device |
| GET | `/api/devices` | List all devices |
| GET | `/api/devices/by-patient/{patientId}` | List all devices for a given patient |
| GET | `/api/devices/{id}` | Get a device by Id (`long`) |
| PUT | `/api/devices/{id}` | Update a device |
| DELETE | `/api/devices/{id}` | Delete a device |

## Supplies (`SuppliesController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/supplies` | Create a supply |
| GET | `/api/supplies` | List all supplies |
| GET | `/api/supplies/by-patient/{patientId}` | List all supplies for a given patient |
| GET | `/api/supplies/{id}` | Get a supply by Id (`long`) |
| PUT | `/api/supplies/{id}` | Update a supply |
| DELETE | `/api/supplies/{id}` | Delete a supply |

## ImagingStudies (`ImagingStudiesController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/imagingstudies` | Create an imaging study |
| GET | `/api/imagingstudies` | List all imaging studies |
| GET | `/api/imagingstudies/by-patient/{patientId}` | List all imaging studies for a given patient |
| GET | `/api/imagingstudies/{id}` | Get an imaging study by Id |
| PUT | `/api/imagingstudies/{id}` | Update an imaging study |
| DELETE | `/api/imagingstudies/{id}` | Delete an imaging study |

## Claims (`ClaimsController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/claims` | Create a claim |
| GET | `/api/claims` | List all claims |
| GET | `/api/claims/by-patient/{patientId}` | List all claims for a given patient |
| GET | `/api/claims/{id}` | Get a claim by Id |
| PUT | `/api/claims/{id}` | Update a claim |
| DELETE | `/api/claims/{id}` | Delete a claim |

## ClaimTransactions (`ClaimTransactionsController`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/claimtransactions` | Create a claim transaction |
| GET | `/api/claimtransactions` | List all claim transactions |
| GET | `/api/claimtransactions/by-claim/{claimId}` | List all claim transactions for a given claim |
| GET | `/api/claimtransactions/{id}` | Get a claim transaction by Id |
| PUT | `/api/claimtransactions/{id}` | Update a claim transaction |
| DELETE | `/api/claimtransactions/{id}` | Delete a claim transaction |

## All 18 domains complete
Every domain now has a full vertical slice (Mapper → Repository → BL → Controller) live and Swagger-documented.
