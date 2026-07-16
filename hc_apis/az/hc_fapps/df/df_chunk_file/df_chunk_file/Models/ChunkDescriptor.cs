namespace df_chunk_file.Models;

/// <summary>Lightweight metadata about one chunk, kept small so orchestration history doesn't bloat with row data.</summary>
public class ChunkDescriptor
{
    /// <summary>Carried over from the originating BulkImportRequest, so a chunk can be traced back
    /// to its parent job if its details are later logged to a database.</summary>
    public string DocumentNumber { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public string ChunkFilePath { get; set; } = string.Empty;
    public int RowCount { get; set; }

    /// <summary>Set if this specific chunk failed while being written (e.g. disk I/O error) — only
    /// this chunk is affected; the rest of the file's chunks are unaffected and still processed.</summary>
    public bool IsNotValid { get; set; }
    public string Message { get; set; } = string.Empty;
}
