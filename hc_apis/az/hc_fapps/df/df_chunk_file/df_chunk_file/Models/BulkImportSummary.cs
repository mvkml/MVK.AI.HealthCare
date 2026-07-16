namespace df_chunk_file.Models;

/// <summary>Final orchestration output — merged results across every chunk.</summary>
public class BulkImportSummary
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string ModuleType { get; set; } = string.Empty;
    public int ChunkCount { get; set; }
    public int TotalRows { get; set; }
    public int InsertedCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
