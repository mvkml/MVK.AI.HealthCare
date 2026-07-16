namespace df_chunk_file.Models;

/// <summary>ActivityTrigger accepts a single input, so the file path and job's DocumentNumber are wrapped together.</summary>
public class SplitFileActivityInput
{
    public string FilePath { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
}
