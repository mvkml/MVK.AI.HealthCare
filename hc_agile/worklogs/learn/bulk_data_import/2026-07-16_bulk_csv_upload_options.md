# Bulk CSV Upload & Large File Transfer — Options Reference

Context: exploring how to import the large Synthea files (`observations.csv` ~20MB/~114K rows,
`claims_transactions.csv` ~51MB/~118K rows) into the Patient API, since the existing batched
CSV import/upsert pattern (built for the other 16 entities) may be too slow as a single
synchronous HTTP request at that row count.

## 1. Bulk data insertion into SQL Server — implementation options

| Approach | How it works | Pros | Cons | Effort |
|---|---|---|---|---|
| **A. Reuse current pattern as-is** | Same streaming CSV reader + 500-row batches via `CreateBatch`/`UpsertBatch` through EF Core, same `/import/upsert` endpoint used for all 16 entities | No new code; keeps per-row error reporting; consistent with rest of API | One long synchronous HTTP request (~228 batches); may hit request timeout or feel slow | None — try it first |
| **B. SqlBulkCopy + staging table + MERGE** | Stream CSV rows into a temp staging table via ADO.NET's `SqlBulkCopy`, then run one `MERGE` SQL statement to upsert staging → target table | Dramatically faster than row-by-row EF Core; built for this volume | Bypasses EF/repository pattern (raw SQL); loses per-row error detail; more code | Medium-High |
| **C. Background job (async endpoint)** | Endpoint returns a job Id immediately, processes the import in a background task/queue, client polls a status endpoint | Solves HTTP-timeout risk directly; good UX; can wrap A or B underneath | Needs job-tracking table, status endpoint, background worker | Medium |
| **D. Client-side chunking** | Split the CSV into smaller pieces before upload, call the existing endpoint once per chunk | Zero backend changes | Just moves the problem to the client; doesn't fix a single-chunk-too-slow case | Low |

**Recommendation:** try A first against the real file to see exactly where it breaks (timeout vs.
slow vs. fine) before building B or C speculatively.

## 2. REST API request size limits (this project's API)

| Setting | Value | Location |
|---|---|---|
| `[RequestSizeLimit(104_857_600)]` | 100 MB | On every controller's `import`/`import/upsert` action |
| `CsvFileValidator.MaxFileSizeBytes` | 100 MB | App-level check, returns friendly `400` if exceeded |

Actual file sizes: `patients.csv` 34KB, `claims.csv` 5.9MB, `observations.csv` 20.4MB,
`claims_transactions.csv` 51.3MB — **all comfortably under the 100MB cap**. Size was never the
blocker for the deferred entities; row-count-driven processing time is.

## 3. Azure Functions / Durable Functions limits (if hosting there instead)

| Aspect | Azure Functions (HTTP trigger) | Durable Functions |
|---|---|---|
| Max request/response payload | 100 MB (hard platform limit, all plans) | No fixed cap — payloads over ~64KB auto-offload to blob storage via the Durable extension |
| Execution timeout | Consumption: default 5 min, max 10 min (`functionTimeout` in `host.json`). Premium/Dedicated: can be unbounded (`-1`) | Not timeout-bound — HTTP "starter" returns immediately (202 + status URL); real work runs in orchestrator/activity functions outside the HTTP request lifetime |
| Fit for this scenario | Same timeout risk as current API on Consumption plan | Exactly the pattern for this: start via HTTP, return job Id, process in background, poll status |

*(Numbers as of this conversation's knowledge — worth re-checking current Microsoft Learn docs
before committing to a number in a design doc; Azure revises these occasionally.)*

## 4. Client → ASP.NET Core large file transfer options

| Approach | How it works | Live progress? | Handles very large files? | Complexity | Best fit |
|---|---|---|---|---|---|
| **A. Standard multipart POST** (current) | Whole file in one `multipart/form-data` request, `IFormFile` binding | No | Limited by `RequestSizeLimit`; whole file buffered | Low | Small/medium files |
| **B. Chunked/resumable upload** (e.g. tus.io) | Client splits file into chunks, uploads via separate requests with an index; server reassembles/processes incrementally | Yes; supports resume | Yes | Medium | Large files, unreliable networks |
| **C. Streaming request body** (`MultipartReader`, no buffering) | Server reads the stream progressively instead of buffering the whole file | No, unless paired with a separate channel | Yes — avoids server memory blowup | Medium | Reduces memory pressure; complements A/B |
| **D. WebSockets** | Persistent connection, file sent as binary frames; server can push messages back on the same connection | Yes, bidirectional | Yes, if chunked | Medium-High | True two-way live status, not just REST |
| **E. SignalR (streaming hub)** | Client streams to a hub method (`IAsyncEnumerable`), built on WS/SSE/long-polling with fallback | Yes, easiest for UI progress | Yes | Medium | Already using SignalR, or want progress UI without hand-rolled WS |
| **F. Direct-to-Blob-Storage upload** | API issues SAS token; client uploads straight to Blob Storage (native huge-file support); calls a small API endpoint with the blob reference | Yes, via Storage SDK progress callbacks | Yes — no practical limit, bypasses API request-size limits entirely | Medium | Very large files; keeps bandwidth off the API server |
| **G. gRPC client-streaming** | File sent as a stream of protobuf messages over HTTP/2 | Server can send progress during the stream | Yes | High | Only if already invested in gRPC elsewhere |

**Recommendation for this project's actual bottleneck** (processing time, not transfer size):
**B (chunked upload)** or **F (direct-to-Blob + background processing)** — not WebSockets.
WebSockets solve a live-connection/bidirectional-messaging problem; the real bottleneck here is
server-side row processing after the file has already arrived, which Durable Functions or a
simple background-job endpoint (Option C from section 1) already solves without a persistent
socket.

## 5. Durable Functions — what it actually solves (and what it doesn't)

Durable Functions is a good fit for "the HTTP request dies before processing finishes" — it
decouples the HTTP call from the long-running work (starter function returns a job Id
immediately, orchestrator/activity functions do the real work outside the request lifetime,
client polls for status). Two nuances:

1. **It solves the timeout problem, not the throughput problem.** The actual insert speed inside
   the activity function still depends on what runs there — EF Core batching (current pattern) vs.
   `SqlBulkCopy`. For both speed and non-blocking behavior, pair Durable Functions with
   `SqlBulkCopy` + staging-table/MERGE (Option B from section 1) inside the activity function,
   not EF Core batching alone.
2. **It means adopting Azure Functions as a runtime for this piece**, not just a code change in
   the existing ASP.NET Core API. The same "return immediately, process in background, poll
   status" pattern can be built *in-process* instead, using a `BackgroundService`/hosted-service
   queue plus a job-status table — no new hosting model, no separate deployment target, same repo.
   Simpler if the project isn't already committed to an Azure Functions component elsewhere.

**Open question to resolve before choosing:** is this project already using (or planning to use)
Azure Functions anywhere else? If not, the in-process background-job approach is the lower-friction
path to the same "don't block the HTTP request" outcome.

## 6. Chosen design: Durable Function as a chunking client (no API changes needed)

Decision made: rather than moving the insert logic itself into a Durable Function activity, the
Durable Function acts purely as a smart **client** in front of the existing ASP.NET Core API. It
reads the large file from a local path, splits it into chunks, and calls the API's existing
`/import/upsert` endpoint once per chunk — exactly like a person uploading several smaller files
via Swagger, just automated and reliable. **This requires zero changes to the API** — Option A
(reuse current pattern) stays exactly as-is; only the caller is new.

### Components

1. **Starter function** (trigger) — kicks off the orchestration, given a local file path + entity
   name as input; returns an instance Id / status-check URL immediately.
2. **Orchestrator function** — coordinates: reads/splits the file into chunks, calls the upload
   activity once per chunk, aggregates the results.
3. **Activity function: "UploadChunkToApi"** — takes one chunk (header + N rows), builds a small
   CSV payload, POSTs it to the existing `POST /api/{entity}/import/upsert` endpoint via a normal
   HTTP client, returns that chunk's `ImportResult`.

### Step-by-step flow

1. Trigger the Durable Function, passing the local CSV file path (e.g.
   `.../documents/patient_details/synthea/observations.csv`) and the target entity name.
2. The starter function begins the orchestration instance with that input and returns immediately
   — the caller doesn't wait for the whole import to finish.
3. The orchestrator function:
   a. Reads the file and splits it into fixed-size chunks (e.g., 5,000–10,000 rows each), keeping
      the CSV header on every chunk so each one is a valid, independently-uploadable file.
   b. For each chunk, calls the `UploadChunkToApi` activity — sequentially, or with limited
      fan-out concurrency to avoid overwhelming the database.
   c. Each activity call is automatically retried by the Durable Functions runtime on transient
      failure (e.g. a network blip) — no custom retry code needed.
   d. The orchestrator collects each chunk's `ImportResult` (`TotalRows`, `InsertedCount`,
      `FailedCount`, `Errors`) and merges them into one final combined result once every chunk is
      done.
4. The original caller polls the orchestration's status endpoint until it reports "Completed",
   then reads the merged final `ImportResult`.

### Why this design works well here

- **No API changes** — the API keeps accepting exactly the size of file it already handles
  comfortably; each chunk stays well under the 100MB cap and completes fast, so the
  timeout/slowness risk from a single giant request disappears entirely.
- **Automatic retry + checkpointing** from the Durable Functions runtime — resilient to transient
  failures without extra code, and the orchestration survives a host restart mid-run.
- **100% reuse** of all existing CSV/import logic, error reporting, and the `ImportResult` shape
  on the API side.
- Clean separation: "how to split and drive a huge file through the API reliably" lives entirely
  in the Durable Function; the API stays exactly as it is today for every entity.

## 7. Static classes vs. DI in Azure Durable Functions — which is correct for enterprise code

Question: does Durable Functions require static classes/methods, or can it be built without them,
and which is the better choice?

**Static is not a requirement.** In the isolated worker model (`Microsoft.Azure.Functions.Worker`,
.NET 8 — what this project uses), trigger methods marked `[Function("Name")]`,
`[OrchestrationTrigger]`, or `[ActivityTrigger]` can be either static methods on a static class, or
instance methods on a regular class. Both are fully supported by the platform.

The default Visual Studio "New Project" template (seen in the scaffolded `Function1.cs` under
`df_chunk_file`) generates a `static class` with `static` methods purely as a quick-demo
convenience — it is not a platform requirement, just the template's default shape.

**The one real constraint is orthogonal to static vs. instance:** the orchestrator function's
body must be deterministic — no direct file I/O, no direct `HttpClient` calls, no
`DateTime.Now`, nothing non-repeatable — because Durable Functions replays that code from history
on every checkpoint. That constraint is about *what APIs are called inside the method*, not
about whether the containing class/method is static.

**For enterprise code, non-static classes with constructor-injected dependencies (DI) are the
better choice**, and this is Microsoft's own documented recommended pattern:

- Define starter, orchestrator, and activity functions as regular (non-static) classes, each
  constructor-injected with the interfaces it needs (e.g. `IFileChunkingService`,
  `IApiUploadClient`, `IImportResultAggregator` from section 6's design).
- The Functions host resolves these classes through the same DI container configured in
  `Program.cs` — the same mechanism ASP.NET Core uses, and consistent with how
  `AI.HealthCare.Patient.API` is already layered (BL → Repository, interface-based, constructor-
  injected).
- This gives testability (mock the interfaces, unit test the Function class directly), matches
  Dependency Inversion, and avoids the rigidity of static dependencies.
- Nuance specific to orchestrators: keep injected dependencies in an orchestrator class limited to
  things safe under replay (e.g. `ILogger`). Actual I/O-bound work (chunking, HTTP upload) belongs
  in **activity** classes, which are not replayed and can freely use injected, real, stateful
  services.

**Conclusion:** the design plan's "no static classes" rule (see
[`durable_function_bulk_import_plan.md`](../../../architecture/design_patterns/durable_function_bulk_import_plan.md))
is correct and should override the scaffolded template's static pattern once `df_chunk_file` is
built out.

## 8. How the status-check URL and orchestrator replay actually work

Question: when a client polls the status-check URL a second time, does that somehow "re-run" the
orchestrator function? Two separate mechanisms are easy to conflate here — they are not the same
thing.

**A. `client.CreateCheckStatusResponseAsync(req, instanceId)`** returns an HTTP `202 Accepted` with
a JSON body of management URLs (`statusQueryGetUri`, `sendEventPostUri`, `terminatePostUri`,
`purgeHistoryDeleteUri`) — not a re-run of any code.

**B. Hitting `statusQueryGetUri` (the "second request")** does **not** call `StartBulkImport` or
`RunOrchestrator` again. That URL is a built-in HTTP endpoint the Durable Functions extension
registers automatically (not code we wrote). It simply reads the orchestration instance's current
state from storage (Azure Table Storage, via `AzureWebJobsStorage`) — `runtimeStatus`, `input`,
`output`, timestamps — and returns it as JSON. It is a passive read; nothing executes.

**C. What actually re-invokes `RunOrchestrator`** is unrelated to polling: every
`await context.CallActivityAsync(...)` sends work to a queue; when that activity finishes, its
result is recorded as an event in the orchestration's history (in storage). That event triggers
the framework's internal dispatcher to **replay** `RunOrchestrator` from the start — deterministically
re-running its code using the recorded history to "fast-forward" through already-completed
awaits — until it reaches the new unfinished point and continues from there.

**Conclusion:** replay is driven entirely by activity completion, completely independent of
whether anyone ever calls the status URL. The orchestration runs to completion on its own even if
no one polls it — polling is purely checking in on state that changes in the background
regardless.

| Action | What it triggers |
|---|---|
| Client GETs `statusQueryGetUri` | Reads stored state only — runs none of our code |
| An activity (`SplitFileIntoChunksActivity`, `UploadChunkToApiActivity`) finishes | Triggers the framework to replay `RunOrchestrator` and continue it |
