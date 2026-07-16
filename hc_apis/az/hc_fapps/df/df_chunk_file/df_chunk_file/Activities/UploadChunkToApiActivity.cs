using df_chunk_file.Models;
using df_chunk_file.Services;
using Microsoft.Azure.Functions.Worker;

namespace df_chunk_file.Activities;

/// <summary>Activity function — real I/O happens here, not in the orchestrator. Delegates to IApiUploadClient.</summary>
public class UploadChunkToApiActivity
{
    private readonly IApiUploadClient _uploadClient;

    public UploadChunkToApiActivity(IApiUploadClient uploadClient)
    {
        _uploadClient = uploadClient;
    }

    [Function(nameof(UploadChunkToApiActivity))]
    public Task<ChunkUploadResult> Run([ActivityTrigger] ChunkUploadActivityInput input)
    {
        return _uploadClient.UploadChunkAsync(input.Chunk, input.ApiEndpoint);
    }
}
