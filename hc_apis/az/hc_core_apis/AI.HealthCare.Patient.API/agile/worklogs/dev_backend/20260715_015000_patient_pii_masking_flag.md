# 💻 Dev Backend Agent — Work Log
## Date: 2026-07-15
## Time: 01:50:00
## Subject: PII response-shape decision resolved — includePii flag with masking

### What Was Done
- Resolved the previously-flagged pending item (#5 in the `patients` pending list — PII fields excluded from `PatientResponse` by default): instead of a separate endpoint, added an `includePii` query-string flag to `Create`/`GetById`/`Update` on `PatientsController`.
- `PatientResponse` now has `Ssn`/`Drivers`/`Passport` properties (previously absent), populated only when `includePii=true`, and always **masked** — `PatientBL.Mask()` keeps the last 4 characters and replaces everything else with `*` (hyphens preserved for readability, e.g. `999-83-4938` → `***-**-4938`). When `includePii` is omitted or `false` (default), these fields are `null`, same as before.
- Added `PatientsModel.IncludePii` (bool) — a Controller-set control flag on the envelope, not part of `PatientRequest` (it controls response shape, not input data).
- Verified live via curl: `includePii=true` on Create/GetById returns masked values (`***-**-4938`, `*****8423`, `******823X`); default omits them as `null`.
- Updated `API_URLS.md` documenting the flag and its behavior.

### Decisions Made
- Chose flag + masking over a separate endpoint, per user's explicit direction — framed as a demo/learning-purpose feature, not real PII protection (noted in code comments).
- `GetAll` was **not** changed — it still returns raw `PatientItem` objects (unmasked, matching `AI.HR.Api`'s own `OcrDocumentBL.GetAll` convention of returning `Item` types directly). This is a pre-existing inconsistency (list endpoint exposes more than single-item endpoint did before this fix) — flagging it as a related but separate follow-up, not fixed in this pass since it wasn't the item the user asked about.

### Pending / Next Steps
- Consider whether `GetAll` should also mask or exclude PII for consistency — separate decision from this one.
- Remaining `patients` pending items: `PatientBLTests`, Controller-level tests, CSV data import, auth.
