using df_chunk_file.Models;
using df_chunk_file.Services;
using Microsoft.Extensions.Options;

namespace df_chunk_file.Tests.Services;

public class FileChunkingServiceTests : IDisposable
{
    private readonly string _workDir;
    private readonly string _tempChunkDirectory;
    private readonly FileChunkingService _service;

    public FileChunkingServiceTests()
    {
        _workDir = Path.Combine(Path.GetTempPath(), "df_chunk_file_tests", Guid.NewGuid().ToString("N"));
        _tempChunkDirectory = Path.Combine(_workDir, "chunks");
        Directory.CreateDirectory(_workDir);

        var options = Options.Create(new BulkImportOptions { TempChunkDirectory = _tempChunkDirectory });
        _service = new FileChunkingService(options);
    }

    public void Dispose()
    {
        if (Directory.Exists(_workDir))
            Directory.Delete(_workDir, recursive: true);
    }

    private string WriteSourceCsv(string header, IEnumerable<string> rows)
    {
        var filePath = Path.Combine(_workDir, $"source_{Guid.NewGuid():N}.csv");
        File.WriteAllText(filePath, string.Join('\n', new[] { header }.Concat(rows)) + "\n");
        return filePath;
    }

    [Fact]
    public async Task SplitIntoChunksAsync_SplitsRowsAcrossMultipleChunks()
    {
        var rows = Enumerable.Range(1, 10).Select(i => $"row{i},value{i}");
        var filePath = WriteSourceCsv("COL1,COL2", rows);

        var chunks = await _service.SplitIntoChunksAsync(filePath, "DOC-1", chunkSize: 4);

        Assert.Equal(3, chunks.Count);
        Assert.Equal([4, 4, 2], chunks.Select(c => c.RowCount));
        Assert.Equal([0, 1, 2], chunks.Select(c => c.ChunkIndex));
        Assert.All(chunks, c => Assert.Equal("DOC-1", c.DocumentNumber));
        Assert.All(chunks, c => Assert.False(c.IsNotValid));
        Assert.All(chunks, c => Assert.True(File.Exists(c.ChunkFilePath)));
    }

    [Fact]
    public async Task SplitIntoChunksAsync_ExactMultipleOfChunkSize_DoesNotProduceEmptyTrailingChunk()
    {
        var rows = Enumerable.Range(1, 10).Select(i => $"row{i}");
        var filePath = WriteSourceCsv("COL1", rows);

        var chunks = await _service.SplitIntoChunksAsync(filePath, "DOC-2", chunkSize: 5);

        Assert.Equal(2, chunks.Count);
        Assert.Equal([5, 5], chunks.Select(c => c.RowCount));
    }

    [Fact]
    public async Task SplitIntoChunksAsync_EachChunkFileKeepsHeaderAndOnlyItsOwnRows()
    {
        var rows = new[] { "row1", "row2", "row3" };
        var filePath = WriteSourceCsv("COL1", rows);

        var chunks = await _service.SplitIntoChunksAsync(filePath, "DOC-3", chunkSize: 2);

        var firstChunkLines = await File.ReadAllLinesAsync(chunks[0].ChunkFilePath);
        Assert.Equal(["COL1", "row1", "row2"], firstChunkLines);

        var secondChunkLines = await File.ReadAllLinesAsync(chunks[1].ChunkFilePath);
        Assert.Equal(["COL1", "row3"], secondChunkLines);
    }

    [Fact]
    public async Task SplitIntoChunksAsync_SkipsBlankLines()
    {
        var filePath = Path.Combine(_workDir, "with_blanks.csv");
        await File.WriteAllTextAsync(filePath, "COL1\nrow1\n\nrow2\n   \nrow3\n");

        var chunks = await _service.SplitIntoChunksAsync(filePath, "DOC-4", chunkSize: 10);

        Assert.Single(chunks);
        Assert.Equal(3, chunks[0].RowCount);
    }

    [Fact]
    public async Task SplitIntoChunksAsync_HeaderOnlyFile_ReturnsNoChunks()
    {
        var filePath = WriteSourceCsv("COL1,COL2", Enumerable.Empty<string>());

        var chunks = await _service.SplitIntoChunksAsync(filePath, "DOC-5", chunkSize: 5000);

        Assert.Empty(chunks);
    }
}
