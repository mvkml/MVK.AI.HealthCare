# TASK004 - Remove HR Leftovers from hc_bigdata

## Linked User Story: US002 - Clean Up Leftover HR Artifacts in hc_bigdata

## Description
`hc_bigdata` currently carries copy-pasted content from the sibling `ai_hr` project: `data/employees.csv` (HR dummy data) and setup docs under `document/setup/` that reference the `hr_bigdata` app name instead of `hc_bigdata`.

## Steps
- [ ] Remove or replace `data/employees.csv` with health-relevant sample data (or delete if unused)
- [ ] Update `document/setup/*.md` to reference `hc_bigdata` and health-specific paths
- [ ] Re-run `01_first_pyspark_query.ipynb` to confirm nothing breaks

## Status: To Do
## Assignee: TBD
## Sprint: sprint_01
