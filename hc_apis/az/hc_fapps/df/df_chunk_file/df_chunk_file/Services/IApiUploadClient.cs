using df_chunk_file.Models;

namespace df_chunk_file.Services;

public interface IApiUploadClient
{
    Task<ChunkUploadResult> UploadChunkAsync(ChunkDescriptor chunk, string apiEndpoint);
}
