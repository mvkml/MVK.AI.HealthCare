namespace df_chunk_file.Models;

/// <summary>ActivityTrigger accepts a single input, so the chunk and target endpoint are wrapped together.</summary>
public class ChunkUploadActivityInput
{
    public ChunkDescriptor Chunk { get; set; } = null!;
    public string ApiEndpoint { get; set; } = string.Empty;
}
