# Azure Durable Function — Bulk CSV Import Client (Design Plan)

## Purpose

Import very large Synthea CSV files (Observations, ClaimTransactions — ~114K–118K rows each)
into the Patient API without any changes to the API itself. The Durable Function is a **client
only**: it reads a file from a local path, splits it into chunks, and uploads each chunk to the
existing `POST /api/{entity}/import/upsert` endpoint via a normal HTTP request. It never touches
SQL Server directly — no `SqlBulkCopy`, no EF Core, no connection string in the Function app at
all. See [`2026-07-16_bulk_csv_upload_options.md`](../../worklogs/learn/bulk_data_import/2026-07-16_bulk_csv_upload_options.md)
section 6 for the reasoning behind this design.

## Roles

| Role | Responsibility |
|---|---|
| **Client (starter function)** | Entry point. Accepts a request (local file path + entity name), starts an orchestration instance, returns immediately with an instance Id / status-check URL. Does no file or HTTP work itself. |
| **Orchestrator** | Coordinates the workflow only — deterministic, replay-safe. Calls activities to split the file, calls activities to upload each chunk (with controlled concurrency), aggregates the per-chunk results into one summary. Contains **no I/O of its own** (Durable Functions requirement: orchestrator code is replayed from history, so file/HTTP calls must live in activities). |
| **Activities** | Do all the actual work — file I/O and HTTP calls — each with a single responsibility, injected via DI. |

## Activity segregation (SRP)

Three activities, each backed by an injected service interface — the Azure Function method itself
is a thin adapter, not where the logic lives:

| Activity Function | Backing service (interface) | Single responsibility |
|---|---|---|
| `SplitFileIntoChunksActivity` | `IFileChunkingService` | Read the CSV file at the given path, split it into ordered chunks (each chunk keeps the CSV header), return lightweight chunk descriptors — not the chunk contents themselves, to keep orchestration history small. |
| `UploadChunkToApiActivity` | `IApiUploadClient` | Given one chunk descriptor, build the chunk's CSV content, POST it as multipart form data to `POST /api/{entity}/import/upsert`, deserialize and return the `ImportResult`. |
| `AggregateResultsActivity` (or inline in orchestrator if kept to pure in-memory merging, which is replay-safe) | `IImportResultAggregator` | Merge N per-chunk `ImportResult`s into one combined `BulkImportSummary` (sums `TotalRows`/`InsertedCount`/`FailedCount`, concatenates `Errors` with chunk context). |

## SOLID application

- **Single Responsibility** — chunking, uploading, and aggregating are three separate services;
  none of them know about the other two. The orchestrator is the only thing that sequences them.
- **Open/Closed** — swapping the chunking strategy (e.g., fixed row count vs. fixed byte size) or
  the upload transport (e.g., adding a retry policy, or switching to a different API shape) means
  adding a new class behind the existing interface — no changes to the orchestrator or the other
  services.
- **Liskov Substitution** — every implementation registered behind `IFileChunkingService`,
  `IApiUploadClient`, `IImportResultAggregator` must be fully substitutable; no implementation
  should require callers to know its concrete type or add special-case handling.
- **Interface Segregation** — each interface exposes only the one method its role needs (e.g.,
  `IFileChunkingService` has no upload method on it); no "fat" shared interface.
- **Dependency Inversion** — the orchestrator and activity functions depend on the three
  interfaces above, never on concrete classes, and never on static helpers. All wiring happens
  through the DI container at startup.

**No static classes anywhere in this design** — every service is a regular class registered in DI
and constructor-injected into the Activity Function class that uses it. `HttpClient` usage inside
`ApiUploadClient` goes through `IHttpClientFactory` (via `AddHttpClient<IApiUploadClient, ApiUploadClient>()`),
not a static/shared `HttpClient` instance, to get correct connection pooling and lifetime
management from the DI container instead.

## Models

| Type | Purpose |
|---|---|
| `BulkImportRequest` | Input to the starter function: `LocalFilePath`, `EntityName`. |
| `ChunkDescriptor` | Output of chunking: `ChunkIndex`, location/range of the chunk's rows, `RowCount`. Deliberately small — avoids bloating orchestration history with full row data. |
| `ChunkUploadResult` | Wraps one chunk's `ImportResult` plus its `ChunkIndex`, for traceability back to which part of the file it came from. |
| `BulkImportSummary` | Final aggregated result: total rows/inserted/failed across all chunks, combined error list (each error still tagged with its originating chunk/row). |

## Configuration (via `IOptions<T>`, not static config access)

| Setting | Purpose |
|---|---|
| `ChunkSize` | Rows per chunk (e.g., 5,000–10,000) — tune based on how the API performs per chunk. |
| `ApiBaseUrl` | Base URL of the Patient API (e.g., `http://localhost:5295/api`), so environment can change without code changes. |
| `MaxConcurrentUploads` | Caps fan-out concurrency for `UploadChunkToApiActivity` calls, so the API/database isn't overwhelmed by too many simultaneous chunk uploads. |

Bound from a `BulkImportOptions` class via the standard `.Configure<BulkImportOptions>(...)`
pattern in `Program.cs`, injected as `IOptions<BulkImportOptions>` wherever needed — no static
`ConfigurationManager`-style access.

## DI registration (Program.cs, isolated worker model)

```csharp
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.Configure<BulkImportOptions>(configuration.GetSection("BulkImport"));

        services.AddHttpClient<IApiUploadClient, ApiUploadClient>();
        services.AddScoped<IFileChunkingService, FileChunkingService>();
        services.AddScoped<IImportResultAggregator, ImportResultAggregator>();
    })
    .Build();
```

## End-to-end flow

1. Something triggers the **starter function** with a `BulkImportRequest` (local file path +
   entity name).
2. Starter function calls `IDurableOrchestrationClient.StartNewAsync(...)`, returns an instance
   Id / status-check URL immediately — caller does not block.
3. **Orchestrator**:
   a. Calls `SplitFileIntoChunksActivity` once — returns a list of `ChunkDescriptor`s.
   b. Calls `UploadChunkToApiActivity` once per chunk (via `context.CallActivityAsync<ChunkUploadResult>(...)`),
      fanned out with concurrency capped by `MaxConcurrentUploads`.
   c. Each activity internally uses `IApiUploadClient` to build the chunk's CSV and POST it to
      the existing `/import/upsert` endpoint — reusing the API's current logic untouched.
   d. Durable Functions automatically retries a failed activity call on transient failure; no
      custom retry code needed.
   e. Once all chunks return, the orchestrator merges the `ChunkUploadResult`s into a
      `BulkImportSummary` via `IImportResultAggregator`.
4. Caller polls the status-check URL until the orchestration reports "Completed", then reads the
   final `BulkImportSummary`.

## What this deliberately does NOT include

- No direct database access anywhere in the Function app.
- No static classes or static helper methods — everything is an interface + DI-registered
  implementation.
- No changes to the existing ASP.NET Core API — `CreateBatch`/`UpsertBatch`/`RunImport` and the
  `/import`/`import/upsert` endpoints stay exactly as they are today for every entity.

## Open items to confirm before implementation starts

- Local file path input mechanism for the starter function (hardcoded per entity vs. passed in
  the trigger request body).
- Concrete chunk size and max concurrency values (start conservative, tune based on real timing
  against `observations.csv`/`claims_transactions.csv`).
- Whether chunk splitting writes temp chunk files to disk (simplest, avoids holding full chunk
  content in orchestration history) or streams directly by byte range.
