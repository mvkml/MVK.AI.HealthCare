namespace df_chunk_file.Utilities;

/// <summary>Deletes a temp chunk file after it has been uploaded, so disk usage doesn't grow across large imports.</summary>
public class TempFileCleaner : ITempFileCleaner
{
    public void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
