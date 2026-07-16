namespace df_chunk_file.Models;

/// <summary>Mirrors the Patient API's ImportRowError JSON shape for a single chunk's response.</summary>
public class ImportRowErrorResponse
{
    public int RowNumber { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
