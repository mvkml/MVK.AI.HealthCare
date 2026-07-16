using df_chunk_file.Models;
using df_chunk_file.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;

namespace df_chunk_file.Activities;

/// <summary>Activity function — real I/O happens here, not in the orchestrator. Delegates to IFileChunkingService.</summary>
public class SplitFileIntoChunksActivity
{
    private readonly IFileChunkingService _chunkingService;
    private readonly BulkImportOptions _options;

    public SplitFileIntoChunksActivity(IFileChunkingService chunkingService, IOptions<BulkImportOptions> options)
    {
        _chunkingService = chunkingService;
        _options = options.Value;
    }

    [Function(nameof(SplitFileIntoChunksActivity))]
    public Task<List<ChunkDescriptor>> Run([ActivityTrigger] SplitFileActivityInput input)
    {
        return _chunkingService.SplitIntoChunksAsync(input.FilePath, input.DocumentNumber, _options.ChunkSize);
    }
}
