using df_chunk_file.Models;
using Microsoft.Extensions.Options;

namespace df_chunk_file.Services;

/// <summary>Reads a large CSV file and splits it into smaller chunk files, each keeping the header row.</summary>
public class FileChunkingService : IFileChunkingService
{
    private readonly BulkImportOptions _options;

    public FileChunkingService(IOptions<BulkImportOptions> options)
    {
        _options = options.Value;
    }

    public async Task<List<ChunkDescriptor>> SplitIntoChunksAsync(string filePath, string documentNumber, int chunkSize)
    {
        Directory.CreateDirectory(_options.TempChunkDirectory);

        var chunks = new List<ChunkDescriptor>();
        using var reader = new StreamReader(filePath);

        var header = await reader.ReadLineAsync();
        if (header is null)
            return chunks;

        var chunkIndex = 0;
        var rowsInChunk = new List<string>();

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            rowsInChunk.Add(line);

            if (rowsInChunk.Count >= chunkSize)
            {
                chunks.Add(await WriteChunkFileAsync(header, rowsInChunk, chunkIndex, documentNumber));
                chunkIndex++;
                rowsInChunk.Clear();
            }
        }

        if (rowsInChunk.Count > 0)
            chunks.Add(await WriteChunkFileAsync(header, rowsInChunk, chunkIndex, documentNumber));

        return chunks;
    }

    /// <summary>Writes one chunk file to disk. If writing this specific chunk fails (e.g. disk I/O error),
    /// only this chunk is marked invalid — the rest of the file's chunks are unaffected.</summary>
    private async Task<ChunkDescriptor> WriteChunkFileAsync(string header, List<string> rows, int chunkIndex, string documentNumber)
    {
        var descriptor = new ChunkDescriptor
        {
            DocumentNumber = documentNumber,
            ChunkIndex = chunkIndex,
            RowCount = rows.Count
        };

        try
        {
            var chunkFilePath = Path.Combine(_options.TempChunkDirectory, $"chunk_{chunkIndex}_{Guid.NewGuid():N}.csv");
            var content = string.Join('\n', new[] { header }.Concat(rows)) + "\n";
            await File.WriteAllTextAsync(chunkFilePath, content);

            descriptor.ChunkFilePath = chunkFilePath;
        }
        catch (Exception ex)
        {
            descriptor.IsNotValid = true;
            descriptor.Message = $"Failed to write chunk {chunkIndex}: {ex.Message}";
        }

        return descriptor;
    }
}
