using df_chunk_file.Models;

namespace df_chunk_file.Services;

public interface IFileChunkingService
{
    Task<List<ChunkDescriptor>> SplitIntoChunksAsync(string filePath, string documentNumber, int chunkSize);
}
