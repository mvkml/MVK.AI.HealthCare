namespace df_chunk_file.Models;

/// <summary>Bound from configuration (BulkImport section) via IOptions&lt;BulkImportOptions&gt;.</summary>
public class BulkImportOptions
{
    public int ChunkSize { get; set; } = 5000;
    public int MaxConcurrentUploads { get; set; } = 5;
    public string TempChunkDirectory { get; set; } = Path.Combine(Path.GetTempPath(), "df_chunk_file");
}
