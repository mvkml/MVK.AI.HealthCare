# Product Backlog

List of all features, enhancements, and fixes for the mvkhc project.
Prioritized by the Product Owner.

| ID    | Title                                                          | Priority | Status      |
|-------|-----------------------------------------------------------------|----------|-------------|
| PB001 | Explore Synthea patient dataset via PySpark (hc_bigdata)         | High     | In Progress |
| PB002 | Remove leftover HR-domain artifacts from hc_bigdata (tech debt)  | High     | To Do       |
| PB003 | Build AI.HealthCare.Patient.API (.NET, patient records)          | High     | To Do       |
| PB004 | Build AI.HC.Api core healthcare API (.NET)                       | High     | To Do       |
| PB005 | Setup clinical document intelligence functions (df_id_extractor, df_notes, fa_upload_doc) | Medium | To Do |
| PB006 | Build hc_ui health web portal (aihcweb, Angular)                 | Medium   | To Do       |
| PB007 | Setup Playwright API test suite (hc_qa)                          | Medium   | To Do       |
| PB008 | Design SQL schema / data source layer (hc_data_source)           | Medium   | To Do       |
| PB009 | Setup DevOps pipelines (hc_ai_ops)                                | Low      | To Do       |
| PB010 | Build health demo app & script (hc_demo)                          | Low      | To Do       |

## Notes
- PB001 is the only item with verified work: PySpark environment is confirmed working and the `allergies` Synthea table has been read and profiled. 18 more Synthea tables (patients, encounters, conditions, medications, procedures, claims, etc.) are present in `hc_bigdata/data/patient_details/synthea/` and not yet explored.
- PB002 exists because `hc_bigdata` currently carries copy-pasted HR-project residue: `data/employees.csv` (HR dummy data), setup docs referencing the `hr_bigdata` app name, and `.claude/settings.json` permissions pointing at `ai_hr\hr_bigdata` paths. Clean up before building further on top of it.
- PB003–PB007 all have folder/project scaffolding in place (layered .NET solutions, Angular app shell, Playwright fixtures/tests dirs) but **zero implementation files** — these are "To Do" from scratch, not partially done.
- PB008–PB010 have empty directory skeletons only, with no defined scope yet.
