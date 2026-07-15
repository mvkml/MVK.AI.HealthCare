# US002 - Clean Up Leftover HR Artifacts in hc_bigdata

**As a** Architect
**I want to** remove HR-project residue from hc_bigdata
**So that** the module is health-domain-clean before more data work is built on it

## Acceptance Criteria
- [ ] Remove or replace `hc_bigdata/data/employees.csv` (HR dummy data, not health domain)
- [ ] Update `hc_bigdata/document/setup/*.md` to reference `hc_bigdata` instead of `hr_bigdata`
- [ ] Update `hc_bigdata/.claude/settings.json` permission paths from `ai_hr\hr_bigdata` to `health\hc_bigdata`
- [ ] Re-verify `01_first_pyspark_query.ipynb` still runs after cleanup

## Priority: High
## Status: To Do
## Sprint: sprint_01
