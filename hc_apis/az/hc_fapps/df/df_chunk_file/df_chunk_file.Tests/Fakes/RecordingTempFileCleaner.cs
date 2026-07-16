using df_chunk_file.Utilities;

namespace df_chunk_file.Tests.Fakes;

/// <summary>Records DeleteFile calls instead of touching disk, so tests can assert cleanup
/// happened (or didn't) without depending on file-system state.</summary>
public class RecordingTempFileCleaner : ITempFileCleaner
{
    public List<string> DeletedPaths { get; } = new();

    public void DeleteFile(string filePath)
    {
        DeletedPaths.Add(filePath);
    }
}
