# TASK005 - Fix hc_bigdata Settings Paths

## Linked User Story: US002 - Clean Up Leftover HR Artifacts in hc_bigdata

## Description
`hc_bigdata/.claude/settings.json` permissions currently hardcode `ai_hr\hr_bigdata` paths instead of the health project's own paths.

## Steps
- [ ] Update permission paths to `health\hc_bigdata`
- [ ] Verify PySpark notebooks still run without permission prompts

## Status: To Do
## Assignee: TBD
## Sprint: sprint_01
