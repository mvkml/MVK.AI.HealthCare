namespace df_chunk_file.Models;

/// <summary>Result of uploading one chunk to the REST API, tagged with the chunk and job it came from.</summary>
public class ChunkUploadResult
{
    public string DocumentNumber { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public int TotalRows { get; set; }
    public int InsertedCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
