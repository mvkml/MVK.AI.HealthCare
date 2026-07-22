# QA — aihcweb demos

One folder per demo, same pattern as `hc_qa/api/ai_hc_api/demo/*`: a standalone script run via
`npm run demo:<name>`, not a Playwright test. Each demo automates a manual setup routine so it
can be reached in one command instead of repeating it by hand — and doubles as the starting point
for the equivalent real test case once it's written.

| Demo | Script | Purpose |
|---|---|---|
| `demo1/` | `npm run demo:login-chat` | Log in as the Doctor demo account and land on the Doctor Chat page |
| `demo2/` | `npm run demo:login-patient-chat` | Log in as the Patient demo account and land on the My Health Assistant page |

**Status:** `demo1` and `demo2` done. `demo3`, ... added as new flows need the same time-saving treatment.
