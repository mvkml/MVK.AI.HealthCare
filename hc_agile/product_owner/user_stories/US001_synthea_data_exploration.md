# US001 - Synthea Patient Data Exploration

**As a** Data Engineer
**I want to** explore and profile the Synthea synthetic patient dataset using PySpark
**So that** we understand the data shape available before building APIs and the UI on top of it

## Acceptance Criteria
- [x] Local PySpark environment verified (Spark 3.5.5, `01_first_pyspark_query.ipynb`)
- [x] Allergies table read and profiled (schema + top 10 rows)
- [ ] Profile remaining core Synthea tables: patients, encounters, conditions, medications, procedures
- [ ] Profile remaining supporting tables: careplans, claims, claims_transactions, devices, imaging_studies, immunizations, observations, organizations, payers, payer_transitions, providers, supplies
- [ ] Document key entity relationships (patient -> encounters -> conditions/medications/procedures) for the Architect

## Priority: High
## Status: In Progress
## Sprint: sprint_01
