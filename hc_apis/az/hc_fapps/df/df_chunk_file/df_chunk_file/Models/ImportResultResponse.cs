namespace df_chunk_file.Models;

/// <summary>Mirrors the Patient API's ImportResult JSON shape returned by POST /import/upsert.</summary>
public class ImportResultResponse
{
    public int TotalRows { get; set; }
    public int InsertedCount { get; set; }
    public int FailedCount { get; set; }
    public List<ImportRowErrorResponse> Errors { get; set; } = new();
}
