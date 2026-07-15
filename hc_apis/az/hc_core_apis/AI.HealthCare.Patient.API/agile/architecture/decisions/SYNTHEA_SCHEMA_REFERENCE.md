# Synthea Dataset — Schema Reference

## Purpose
This is a plain-language data dictionary for the 17 source CSV files in `documents/patient_details/synthea/`. It explains **what each file represents and what its columns mean**, and maps each CSV to its proposed database table name.

For column types, PK/FK design, and open decisions (surrogate keys, normalization, DB engine), see [SQL_DESIGN_synthea_import.md](SQL_DESIGN_synthea_import.md) — this doc is the "what is this data" companion to that "how do we store it" doc.

## Dataset
[Synthea](https://synthetichealth.github.io/synthea/) — synthetic patient/EHR generator. All patients, encounters, conditions, etc. are fake but structurally realistic. One CSV file per entity type; files are linked to each other by UUID-style IDs (e.g. `PATIENT`, `ENCOUNTER` columns).

## Core relationship
Every clinical fact traces back to a `PATIENT`, and most trace through an `ENCOUNTER` (a single visit/interaction). Financial facts additionally trace through a `PAYER` (insurer) and `PROVIDER`/`ORGANIZATION`.

```
patients ─┬─< encounters ─┬─< conditions, allergies, medications, procedures,
          │                │   immunizations, observations, devices, supplies,
          │                │   imaging_studies, careplans
          │                └─< claims ─< claims_transactions
          ├─< payer_transitions >─ payers
          └─< (patient identity/demographics only)

organizations ─< providers
organizations, providers, payers ─< encounters, claims
```

---

## CSV → Table Reference

### 1. `patients.csv` → `patients`
One row per person. The root entity everything else hangs off.
| Column | Meaning |
|---|---|
| Id | Patient's unique identifier (UUID) |
| BIRTHDATE / DEATHDATE | Life span; DEATHDATE blank if still alive |
| SSN / DRIVERS / PASSPORT | Government ID numbers — **PII** |
| PREFIX/FIRST/MIDDLE/LAST/SUFFIX/MAIDEN | Name parts |
| MARITAL | Marital status code |
| RACE / ETHNICITY / GENDER | Demographics |
| BIRTHPLACE | Free-text city/state/country of birth |
| ADDRESS/CITY/STATE/COUNTY/FIPS/ZIP/LAT/LON | Current residence |
| HEALTHCARE_EXPENSES / HEALTHCARE_COVERAGE | Lifetime totals: what patient paid vs. what insurance paid |
| INCOME | Annual income (used by Synthea to simulate coverage eligibility) |

### 2. `organizations.csv` → `organizations`
Healthcare facilities (clinics, hospitals) where care is delivered.
| Column | Meaning |
|---|---|
| Id | Organization's unique identifier |
| NAME/ADDRESS/CITY/STATE/ZIP/LAT/LON | Facility identity and location |
| PHONE | Contact number |
| REVENUE | Simulated total revenue |
| UTILIZATION | Count of encounters/procedures handled |

### 3. `providers.csv` → `providers`
Individual clinicians (doctors), each employed by one organization.
| Column | Meaning |
|---|---|
| Id | Provider's unique identifier |
| ORGANIZATION | FK → organizations.Id |
| NAME / GENDER / SPECIALITY | Clinician identity and specialty |
| ADDRESS/CITY/STATE/ZIP/LAT/LON | Practice location (usually matches org) |
| ENCOUNTERS / PROCEDURES | Simulated lifetime counts handled by this provider |

### 4. `payers.csv` → `payers`
Insurance companies (including "NO_INSURANCE" as a payer).
| Column | Meaning |
|---|---|
| Id | Payer's unique identifier |
| NAME / OWNERSHIP | e.g. "Medicare", "GOVERNMENT" |
| ADDRESS/CITY/STATE_HEADQUARTERED/ZIP/PHONE | Contact info |
| AMOUNT_COVERED / AMOUNT_UNCOVERED / REVENUE | Aggregate financials |
| COVERED_*/UNCOVERED_* (ENCOUNTERS, MEDICATIONS, PROCEDURES, IMMUNIZATIONS) | Aggregate counts of what this payer did/didn't cover |
| UNIQUE_CUSTOMERS | Distinct patients ever covered |
| QOLS_AVG | Average quality-of-life score across covered patients |
| MEMBER_MONTHS | Total months of coverage across all members |

### 5. `payer_transitions.csv` → `payer_transitions`
History of which insurance plan a patient held over time (patients can switch payers).
| Column | Meaning |
|---|---|
| PATIENT | FK → patients.Id |
| MEMBERID | Patient's member/policy number with that payer |
| START_DATE / END_DATE | Coverage period |
| PAYER / SECONDARY_PAYER | FK → payers.Id (primary and optional secondary insurance) |
| PLAN_OWNERSHIP | e.g. "Self", "Spouse", "Guardian" |
| OWNER_NAME | Name of the policy owner if not the patient |

*(No `Id` column in source — composite key candidate: PATIENT + START_DATE + PAYER.)*

### 6. `encounters.csv` → `encounters`
A single visit/interaction between a patient and the healthcare system. **Most other clinical tables reference this.**
| Column | Meaning |
|---|---|
| Id | Encounter's unique identifier |
| START / STOP | Visit start/end timestamp |
| PATIENT | FK → patients.Id |
| ORGANIZATION | FK → organizations.Id |
| PROVIDER | FK → providers.Id |
| PAYER | FK → payers.Id (insurance active at time of visit) |
| ENCOUNTERCLASS | e.g. "wellness", "ambulatory", "emergency", "inpatient" |
| CODE / DESCRIPTION | SNOMED code for the encounter type |
| BASE_ENCOUNTER_COST | List price of the visit itself |
| TOTAL_CLAIM_COST | Total cost of everything billed during this visit (visit + procedures + meds etc.) |
| PAYER_COVERAGE | Amount insurance paid |
| REASONCODE / REASONDESCRIPTION | Clinical reason for the visit, if any |

### 7. `conditions.csv` → `conditions`
Diagnoses / health conditions a patient has or had.
| Column | Meaning |
|---|---|
| START / STOP | When condition was diagnosed / resolved (blank STOP = ongoing) |
| PATIENT | FK → patients.Id |
| ENCOUNTER | FK → encounters.Id (visit where diagnosed) |
| SYSTEM | Coding system used (e.g. SNOMED-CT URL) |
| CODE / DESCRIPTION | The condition itself, e.g. "Acute bronchitis (disorder)" |

*(No `Id` column in source.)*

### 8. `allergies.csv` → `allergies` (+ candidate child table `allergy_reactions`)
Allergies a patient has, with up to two recorded reactions.
| Column | Meaning |
|---|---|
| START / STOP | When allergy was recorded / resolved |
| PATIENT | FK → patients.Id |
| ENCOUNTER | FK → encounters.Id |
| CODE / SYSTEM / DESCRIPTION | The allergen (e.g. "Allergic disposition") |
| TYPE | "allergy" or "intolerance" |
| CATEGORY | e.g. "environment", "medication", "food" |
| REACTION1/DESCRIPTION1/SEVERITY1 | First recorded reaction and its severity |
| REACTION2/DESCRIPTION2/SEVERITY2 | Second recorded reaction, if any (mostly blank) |

*(No `Id` column. REACTION1/REACTION2 pair is the normalization candidate — flatten into one `allergy_reactions` row-per-reaction table instead of two fixed slot-columns.)*

### 9. `medications.csv` → `medications`
Drugs prescribed to a patient.
| Column | Meaning |
|---|---|
| START / STOP | Prescription period |
| PATIENT | FK → patients.Id |
| PAYER | FK → payers.Id (insurance covering this prescription) |
| ENCOUNTER | FK → encounters.Id |
| CODE / DESCRIPTION | The drug (e.g. "Acetaminophen 325 MG Oral Tablet") |
| BASE_COST | List price per dispense |
| PAYER_COVERAGE | Amount insurance paid |
| DISPENSES | Number of times refilled/dispensed |
| TOTALCOST | BASE_COST × DISPENSES roughly |
| REASONCODE / REASONDESCRIPTION | Condition this medication treats |

*(No `Id` column.)*

### 10. `careplans.csv` → `careplans`
Ongoing treatment plans (e.g. "Respiratory therapy") assigned to a patient.
| Column | Meaning |
|---|---|
| Id | Careplan's unique identifier |
| START / STOP | Plan active period |
| PATIENT | FK → patients.Id |
| ENCOUNTER | FK → encounters.Id (visit where plan was created) |
| CODE / DESCRIPTION | The plan type |
| REASONCODE / REASONDESCRIPTION | Condition the plan addresses |

### 11. `procedures.csv` → `procedures`
Medical procedures performed on a patient.
| Column | Meaning |
|---|---|
| START / STOP | Procedure timing |
| PATIENT | FK → patients.Id |
| ENCOUNTER | FK → encounters.Id |
| SYSTEM / CODE / DESCRIPTION | The procedure performed |
| BASE_COST | List price |
| REASONCODE / REASONDESCRIPTION | Condition the procedure addresses |

*(No `Id` column.)*

### 12. `immunizations.csv` → `immunizations`
Vaccines administered.
| Column | Meaning |
|---|---|
| DATE | When administered |
| PATIENT | FK → patients.Id |
| ENCOUNTER | FK → encounters.Id |
| CODE / DESCRIPTION | The vaccine (e.g. "Influenza split virus trivalent PF") |
| BASE_COST | List price |

*(No `Id` column.)*

### 13. `observations.csv` → `observations`
Clinical measurements/readings recorded during a visit (vitals, labs).
| Column | Meaning |
|---|---|
| DATE | When measured |
| PATIENT | FK → patients.Id |
| ENCOUNTER | FK → encounters.Id |
| CATEGORY | e.g. "vital-signs", "laboratory" |
| CODE / DESCRIPTION | What was measured (e.g. "Body Height") |
| VALUE / UNITS | The measured value and its unit |
| TYPE | Value's data type: "numeric", "text", etc. |

*(No `Id` column. Highest expected row volume of all tables — one row per measurement per visit.)*

### 14. `devices.csv` → `devices`
Medical devices attached to/implanted in a patient.
| Column | Meaning |
|---|---|
| START / STOP | Time device was in use |
| PATIENT | FK → patients.Id |
| ENCOUNTER | FK → encounters.Id |
| CODE / DESCRIPTION | The device (e.g. "Dental x-ray system") |
| UDI | Unique Device Identifier (barcode-style string) |

*(No `Id` column.)*

### 15. `supplies.csv` → `supplies`
Medical supplies used during a visit.
| Column | Meaning |
|---|---|
| DATE | When used |
| PATIENT | FK → patients.Id |
| ENCOUNTER | FK → encounters.Id |
| CODE / DESCRIPTION | The supply item |
| QUANTITY | Units used |

*(No `Id` column.)*

### 16. `imaging_studies.csv` → `imaging_studies`
Radiology/imaging studies (X-ray, MRI, etc.) performed.
| Column | Meaning |
|---|---|
| Id | Study's unique identifier |
| DATE | When performed |
| PATIENT | FK → patients.Id |
| ENCOUNTER | FK → encounters.Id |
| SERIES_UID / INSTANCE_UID | DICOM series/instance identifiers |
| BODYSITE_CODE / BODYSITE_DESCRIPTION | Body part imaged |
| MODALITY_CODE / MODALITY_DESCRIPTION | Imaging technique (e.g. "DX" = Digital Radiography) |
| SOP_CODE / SOP_DESCRIPTION | DICOM SOP class (image storage format) |
| PROCEDURE_CODE | Linked procedure code |

### 17. `claims.csv` → `claims`
Insurance claims — one per billable episode, can bundle up to 8 diagnoses.
| Column | Meaning |
|---|---|
| Id | Claim's unique identifier |
| PATIENTID | FK → patients.Id |
| PROVIDERID | FK → providers.Id |
| PRIMARYPATIENTINSURANCEID / SECONDARYPATIENTINSURANCEID | FK → payers.Id |
| DEPARTMENTID / PATIENTDEPARTMENTID | Internal department codes |
| DIAGNOSIS1..DIAGNOSIS8 | Up to 8 diagnosis codes billed on this claim |
| REFERRINGPROVIDERID / SUPERVISINGPROVIDERID | FK → providers.Id |
| APPOINTMENTID | FK → encounters.Id |
| CURRENTILLNESSDATE / SERVICEDATE | Clinical dates |
| STATUS1/STATUS2/STATUSP | Billing status for primary/secondary insurance and patient portion |
| OUTSTANDING1/OUTSTANDING2/OUTSTANDINGP | Amount still owed by each party |
| LASTBILLEDDATE1/2/P | Last billed date per party |
| HEALTHCARECLAIMTYPEID1/2 | Claim type codes |

*(DIAGNOSIS1-8 is the normalization candidate — flatten into a `claim_diagnoses` child table.)*

### 18. `claims_transactions.csv` → `claims_transactions`
Individual line-item transactions (charges, payments, adjustments) against a claim.
| Column | Meaning |
|---|---|
| ID | Transaction's unique identifier |
| CLAIMID | FK → claims.Id |
| CHARGEID | Sequence number of the charge line within the claim |
| PATIENTID | FK → patients.Id |
| TYPE | "CHARGE", "PAYMENT", "ADJUSTMENT", "TRANSFERIN"/"TRANSFEROUT" |
| AMOUNT | Transaction amount |
| METHOD | Payment method, if applicable |
| FROMDATE / TODATE | Service period this line covers |
| PLACEOFSERVICE | FK → organizations.Id |
| PROCEDURECODE | Billed procedure code |
| MODIFIER1/MODIFIER2 | Billing modifiers |
| DIAGNOSISREF1-4 | Which of the claim's DIAGNOSIS1-8 slots this line applies to |
| UNITS | Quantity billed |
| DEPARTMENTID | Internal department code |
| NOTES | Free-text description |
| UNITAMOUNT | Amount per unit |
| TRANSFEROUTID / TRANSFERTYPE | Linked transfer transaction, if this is a transfer |
| PAYMENTS / ADJUSTMENTS / TRANSFERS / OUTSTANDING | Running financial breakdown |
| APPOINTMENTID | FK → encounters.Id |
| LINENOTE | Free-text line note |
| PATIENTINSURANCEID | FK → payer_transitions (which coverage applied) |
| FEESCHEDULEID | Fee schedule reference |
| PROVIDERID / SUPERVISINGPROVIDERID | FK → providers.Id |

---

## Quick Table Index
| # | CSV File | Table Name | Grain |
|---|---|---|---|
| 1 | patients.csv | `patients` | one row per person |
| 2 | organizations.csv | `organizations` | one row per facility |
| 3 | providers.csv | `providers` | one row per clinician |
| 4 | payers.csv | `payers` | one row per insurer |
| 5 | payer_transitions.csv | `payer_transitions` | one row per coverage period |
| 6 | encounters.csv | `encounters` | one row per visit |
| 7 | conditions.csv | `conditions` | one row per diagnosis |
| 8 | allergies.csv | `allergies` | one row per allergy |
| 9 | medications.csv | `medications` | one row per prescription |
| 10 | careplans.csv | `careplans` | one row per treatment plan |
| 11 | procedures.csv | `procedures` | one row per procedure performed |
| 12 | immunizations.csv | `immunizations` | one row per vaccine given |
| 13 | observations.csv | `observations` | one row per measurement/reading |
| 14 | devices.csv | `devices` | one row per device usage period |
| 15 | supplies.csv | `supplies` | one row per supply usage |
| 16 | imaging_studies.csv | `imaging_studies` | one row per imaging study |
| 17 | claims.csv | `claims` | one row per billable episode |
| 18 | claims_transactions.csv | `claims_transactions` | one row per billing line item |

See [SQL_DESIGN_synthea_import.md](SQL_DESIGN_synthea_import.md) for column data types, primary/foreign key design, and the 5 open decisions awaiting sign-off.
