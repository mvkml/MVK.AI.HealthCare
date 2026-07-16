using System.Net;
using df_chunk_file.Activities;
using df_chunk_file.Models;
using df_chunk_file.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace df_chunk_file;

/// <summary>Client (HTTP starter) and orchestrator kept together, per design: this class only coordinates —
/// it does no file or HTTP I/O itself. All I/O lives in the Activity classes.</summary>
public class BulkImportOrchestration
{
    private readonly IImportResultAggregator _aggregator;
    private readonly BulkImportOptions _options;

    public BulkImportOrchestration(IImportResultAggregator aggregator, IOptions<BulkImportOptions> options)
    {
        _aggregator = aggregator;
        _options = options.Value;
    }

    /// <summary>Client entry point. Accepts { filePath, moduleType, apiEndpoint }, starts the orchestration,
    /// and returns immediately with a status-check URL (does not wait for the import to finish).</summary>
    [Function(nameof(StartBulkImport))]
    public async Task<HttpResponseData> StartBulkImport(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(BulkImportOrchestration));
        var request = await req.ReadFromJsonAsync<BulkImportRequest>();

        if (request is null || string.IsNullOrWhiteSpace(request.FilePath) || string.IsNullOrWhiteSpace(request.ApiEndpoint))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("filePath and apiEndpoint are required; moduleType is recommended for labeling results.");
            return badResponse;
        }

        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(RunOrchestrator), request);
        logger.LogInformation(
            "Started bulk import orchestration {InstanceId} for module {ModuleType}, file {FilePath}",
            instanceId, request.ModuleType, request.FilePath);

        // The returned response includes a status-check URL. Polling it returns runtimeStatus
        // (Running / Completed / Failed) — once Completed, the response body's "output" is the
        // BulkImportSummary returned by RunOrchestrator below.
        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }

    /// <summary>Orchestrator — coordinates only. Must stay deterministic/replay-safe: no direct file or
    /// HTTP I/O here, only context.CallActivityAsync calls and pure in-memory computation.</summary>
    [Function(nameof(RunOrchestrator))]
    public async Task<BulkImportSummary> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var request = context.GetInput<BulkImportRequest>()!;
        var logger = context.CreateReplaySafeLogger(nameof(BulkImportOrchestration));

        var chunks = await context.CallActivityAsync<List<ChunkDescriptor>>(
            nameof(SplitFileIntoChunksActivity),
            new SplitFileActivityInput { FilePath = request.FilePath, DocumentNumber = request.DocumentNumber });

        logger.LogInformation("Split {ModuleType} file into {ChunkCount} chunks", request.ModuleType, chunks.Count);

        var maxConcurrency = _options.MaxConcurrentUploads > 0 ? _options.MaxConcurrentUploads : 5;
        var chunkResults = new List<ChunkUploadResult>();

        foreach (var batch in chunks.Chunk(maxConcurrency))
        {
            var batchTasks = batch.Select(chunk => context.CallActivityAsync<ChunkUploadResult>(
                nameof(UploadChunkToApiActivity),
                new ChunkUploadActivityInput { Chunk = chunk, ApiEndpoint = request.ApiEndpoint }));

            chunkResults.AddRange(await Task.WhenAll(batchTasks));
        }

        return _aggregator.Aggregate(request.DocumentNumber, request.ModuleType, chunkResults);
    }
}
