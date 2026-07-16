# Large-file demo capacity — what we've actually handled

Reference numbers for the demo: exact size/row count of every Synthea CSV, and how each
one is currently uploaded (direct Playwright demo vs. the `df_chunk_file` Durable Function
client vs. not yet possible). Sorted biggest → smallest so the "how big can we go" story is
easy to tell live.

## All 18 entity files

| # | File | Size (bytes) | Size (MB) | Size (GB) | Rows | Handled via |
|---|---|---:|---:|---:|---:|---|
| 1 | `claims_transactions.csv` | 53,766,754 | 51.28 MB | 0.050 GB | 118,466 | ✅ **Done via `df_chunk_file`** — see results below |
| 2 | `observations.csv` | 20,383,215 | 19.44 MB | 0.019 GB | 114,342 | ✅ **Done via `df_chunk_file`** (Durable Function chunking client) — see results below |
| 3 | `claims.csv` | 5,910,165 | 5.64 MB | 0.0055 GB | 15,390 | ✅ Direct Playwright demo (single-request upsert) |
| 4 | `procedures.csv` | 4,558,047 | 4.35 MB | — | 20,527 | ✅ Direct Playwright demo |
| 5 | `imaging_studies.csv` | 3,699,304 | 3.53 MB | — | 10,524 | ✅ Direct Playwright demo |
| 6 | `encounters.csv` | 2,397,435 | 2.29 MB | — | 7,210 | ✅ Direct Playwright demo |
| 7 | `medications.csv` | 2,175,024 | 2.07 MB | — | 8,180 | ✅ Direct Playwright demo |
| 8 | `conditions.csv` | 747,045 | 0.71 MB | — | 4,748 | ✅ Direct Playwright demo |
| 9 | `payer_transitions.csv` | 726,461 | 0.69 MB | — | 4,368 | ✅ Direct Playwright demo |
| 10 | `supplies.csv` | 561,796 | 0.54 MB | — | 3,975 | ✅ Direct Playwright demo |
| 11 | `immunizations.csv` | 226,988 | 0.22 MB | — | 1,625 | ✅ Direct Playwright demo |
| 12 | `devices.csv` | 174,980 | 0.17 MB | — | 768 | ✅ Direct Playwright demo |
| 13 | `careplans.csv` | 66,540 | 0.06 MB | — | 334 | ✅ Direct Playwright demo |
| 14 | `providers.csv` | 53,262 | 0.05 MB | — | 283 | ✅ Direct Playwright demo |
| 15 | `organizations.csv` | 44,364 | 0.04 MB | — | 283 | ✅ Direct Playwright demo |
| 16 | `patients.csv` | 34,299 | 0.03 MB | — | 113 | ✅ Direct Playwright demo |
| 17 | `allergies.csv` | 15,552 | 0.01 MB | — | 86 | ✅ Direct Playwright demo |
| 18 | `payers.csv` | 1,783 | 0.002 MB | — | 10 | ✅ Direct Playwright demo |

Row counts exclude the CSV header row. Sizes measured directly from disk (`stat`), not estimated.

## Demo-ready headline numbers

| Metric | Value |
|---|---|
| **Biggest file actually processed end-to-end** | `claims_transactions.csv` — 51.28 MB / 118,466 rows (via `df_chunk_file`) |
| **All 18 entity files** | **100% now processed** — every file in the dataset has a working import path |
| **Combined size, all 18 files** | 95,543,014 bytes ≈ **91.13 MB** (0.089 GB) — all of it now importable |
| **Combined rows, all 18 files** | 311,232 rows |
| Single-request ceiling vs. chunked-client ceiling | Direct Playwright demo tops out at `claims.csv` (5.64 MB); `df_chunk_file` handled a file **9.1x bigger** (`claims_transactions.csv`, 51.28 MB) with 0 failures |

## Why this matters for the demo

- Every file handled directly (all 16 smaller entities) tops out around **5–6 MB**. That's
  comfortably inside a single HTTP POST — no chunking needed, which is why the direct
  Playwright demo pattern worked fine for them.
- `observations.csv` and `claims_transactions.csv` are **3.5x and 9x bigger** than anything
  in that group, and together account for the majority of the total dataset by size. They're
  the reason `df_chunk_file` (the Durable Function chunking client) was built in the first
  place — a single synchronous request against these isn't the right shape.

## `observations.csv` — first real run through `df_chunk_file` (2026-07-16)

The `Observation` entity only had CRUD before today, no CSV import at all. Built the missing
import endpoint (`POST /api/observations/import`, insert-only — observations are an
append-only historical log with no natural unique key), then ran the full file through
`df_chunk_file` instead of a direct request, since this is exactly the file it was built for.

| Metric | Value |
|---|---|
| File | `observations.csv` — 19.44 MB / 114,342 rows |
| Chunk size configured | 5,000 rows/chunk (`BulkImport:ChunkSize`) |
| Chunks produced | **23** |
| Max concurrent uploads | 5 (`BulkImport:MaxConcurrentUploads`) |
| Rows inserted | **114,342** |
| Rows failed | **0** |
| Wall-clock time | Orchestration was already `Completed` on the first status poll (~5s after triggering) — chunked+concurrent upload finished markedly faster than a single direct request |
| Verified | Queried SQL Server directly: `COUNT(*) = 114342`, and 2,994 rows correctly have `NULL` `EncounterId` (matches the CSV's DALY/QALY/QOLS patient-level summary rows) |

**Two real schema bugs found and fixed along the way** (both via a plain direct-request test
*before* switching to `df_chunk_file` — the direct test is what surfaced them):
1. `EncounterId` was a required `Guid` FK, but ~2,994 rows (yearly QALY/DALY/QOLS scores) have
   no encounter at all — not tied to a clinical visit. Fixed by making it nullable (`Guid?`),
   small EF migration, no data loss.
2. `Value` column was `nvarchar(50)`, but free-text survey answers run up to 101 characters.
   Widened to `nvarchar(250)` to match the convention used for other free-text columns.

**Operational lesson**: an interrupted direct-`curl` request still completed in the background
after being "rejected," causing a full duplicate import (228,684 rows instead of 114,342) before
the `df_chunk_file` run. Caught by re-verifying the row count in SQL Server rather than trusting
the first count seen, and cleaned up with `TRUNCATE TABLE` before the real run. Lesson: always
re-verify row counts against the DB directly after any interrupted/retried request, don't assume
a killed foreground command didn't still finish server-side.

## `claims_transactions.csv` — the biggest file, via `df_chunk_file` (2026-07-16)

Same situation as Observation: `ClaimTransaction` had full CRUD already but no CSV import. Built
`POST /api/claimtransactions/import` (insert-only — the CSV's `ID` column is a genuine unique key,
confirmed zero duplicates across all 118,466 rows), tested with a 1,000-row sample first, then ran
the full file through `df_chunk_file`.

| Metric | Value |
|---|---|
| File | `claims_transactions.csv` — 51.28 MB / 118,466 rows (the largest file in the dataset) |
| Chunks produced | **24** (same 5,000-row chunk size) |
| Rows inserted | **118,466** |
| Rows failed | **0** |
| Wall-clock time | ~100–110s from trigger to `Completed` (11 polls at 10s intervals) |
| Verified | SQL Server directly: `COUNT(*) = 118466`; 63,470 rows correctly have `NULL` `Amount` (PAYMENT/ADJUSTMENT rows carry no charge amount); all 118,466 rows have a populated `FeeScheduleId`; 113,500 have a `PatientInsuranceId` |

**Two more real schema bugs found and fixed** (via the 1,000-row sample test, before committing
to the full 118K-row run):
1. `Amount` was a required `decimal`, but 63,470 of 118,466 rows (PAYMENT/ADJUSTMENT transaction
   types) legitimately have no amount. Fixed by making it nullable (`decimal?`).
2. `PatientInsuranceId` and `FeeScheduleId` had their CLR types **swapped** relative to the actual
   CSV data — `PATIENTINSURANCEID` is always a GUID, `FEESCHEDULEID` is always a small int, but the
   entity had them typed the other way around (`int?` / `Guid?`). This one couldn't be fixed with a
   plain `ALTER COLUMN` — SQL Server has no implicit cast between `int` and `uniqueidentifier`, so
   the migration had to drop and re-add both columns (safe here since the table was still empty).
3. `Notes` column was `nvarchar(250)`; some medication-related notes run up to 269 characters.
   Widened to `nvarchar(400)`.

**All 18 entities now have a working import path — this closes out the "big file" gap** that
motivated `df_chunk_file` in the first place. Total dataset (91.13 MB / 311,232 rows across all
18 Synthea CSVs) is now fully importable, either directly (16 smaller entities) or via the
chunking client (`observations.csv`, `claims_transactions.csv`).

See also: [`2026-07-16_bulk_csv_upload_options.md`](2026-07-16_bulk_csv_upload_options.md) (design
rationale) and [`durable_function_bulk_import_plan.md`](../../../architecture/design_patterns/durable_function_bulk_import_plan.md)
(implementation plan).
