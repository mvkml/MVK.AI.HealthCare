using df_chunk_file.Models;
using df_chunk_file.Services;

namespace df_chunk_file.Tests.Services;

public class ImportResultAggregatorTests
{
    private readonly ImportResultAggregator _aggregator = new();

    [Fact]
    public void Aggregate_MultipleChunks_SumsCountsAcrossChunks()
    {
        var chunkResults = new List<ChunkUploadResult>
        {
            new() { ChunkIndex = 0, TotalRows = 500, InsertedCount = 500, FailedCount = 0 },
            new() { ChunkIndex = 1, TotalRows = 500, InsertedCount = 480, FailedCount = 20 },
            new() { ChunkIndex = 2, TotalRows = 300, InsertedCount = 300, FailedCount = 0 },
        };

        var summary = _aggregator.Aggregate("DOC-1", "Observation", chunkResults);

        Assert.Equal("DOC-1", summary.DocumentNumber);
        Assert.Equal("Observation", summary.ModuleType);
        Assert.Equal(3, summary.ChunkCount);
        Assert.Equal(1300, summary.TotalRows);
        Assert.Equal(1280, summary.InsertedCount);
        Assert.Equal(20, summary.FailedCount);
    }

    [Fact]
    public void Aggregate_NoChunks_ReturnsZeroedSummary()
    {
        var summary = _aggregator.Aggregate("DOC-2", "ClaimTransaction", new List<ChunkUploadResult>());

        Assert.Equal(0, summary.ChunkCount);
        Assert.Equal(0, summary.TotalRows);
        Assert.Equal(0, summary.InsertedCount);
        Assert.Equal(0, summary.FailedCount);
        Assert.Empty(summary.Errors);
    }

    [Fact]
    public void Aggregate_ErrorsFromMultipleChunks_AreMergedInOrder()
    {
        var chunkResults = new List<ChunkUploadResult>
        {
            new() { ChunkIndex = 0, Errors = ["Chunk 0 row -1: batch failed"] },
            new() { ChunkIndex = 1, Errors = ["Chunk 1 row 42: bad guid", "Chunk 1 row 43: bad guid"] },
        };

        var summary = _aggregator.Aggregate("DOC-3", "Observation", chunkResults);

        Assert.Equal(3, summary.Errors.Count);
        Assert.Equal("Chunk 0 row -1: batch failed", summary.Errors[0]);
        Assert.Equal("Chunk 1 row 42: bad guid", summary.Errors[1]);
        Assert.Equal("Chunk 1 row 43: bad guid", summary.Errors[2]);
    }
}
