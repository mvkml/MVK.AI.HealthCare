namespace df_chunk_file.Models;

/// <summary>Input message for the HTTP starter — the file to chunk and the API to upload each chunk to.</summary>
public class BulkImportRequest
{
    /// <summary>Caller-supplied reference number for this import job, carried through to every
    /// chunk/result so it can be traced back if chunk details are later logged to a database.</summary>
    public string DocumentNumber { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ModuleType { get; set; } = string.Empty;
    public string ApiEndpoint { get; set; } = string.Empty;
}
