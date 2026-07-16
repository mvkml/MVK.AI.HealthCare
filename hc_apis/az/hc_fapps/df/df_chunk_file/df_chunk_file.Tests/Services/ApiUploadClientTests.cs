using System.Net;
using System.Net.Http.Json;
using df_chunk_file.Models;
using df_chunk_file.Services;
using df_chunk_file.Tests.Fakes;

namespace df_chunk_file.Tests.Services;

public class ApiUploadClientTests : IDisposable
{
    private readonly string _workDir;

    public ApiUploadClientTests()
    {
        _workDir = Path.Combine(Path.GetTempPath(), "df_chunk_file_tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_workDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_workDir))
            Directory.Delete(_workDir, recursive: true);
    }

    private static ApiUploadClient BuildClient(FakeHttpMessageHandler handler, RecordingTempFileCleaner cleaner) =>
        new(new HttpClient(handler), cleaner);

    [Fact]
    public async Task UploadChunkAsync_ChunkMarkedInvalid_SkipsHttpCallAndReturnsFailureResult()
    {
        var handler = new FakeHttpMessageHandler(_ => throw new InvalidOperationException("HTTP should not be called for an invalid chunk"));
        var cleaner = new RecordingTempFileCleaner();
        var client = BuildClient(handler, cleaner);

        var chunk = new ChunkDescriptor
        {
            DocumentNumber = "DOC-9",
            ChunkIndex = 2,
            RowCount = 500,
            IsNotValid = true,
            Message = "Failed to write chunk 2: disk full",
        };

        var result = await client.UploadChunkAsync(chunk, "http://fake-endpoint.test/import");

        Assert.Equal(0, handler.CallCount);
        Assert.Empty(cleaner.DeletedPaths);
        Assert.Equal("DOC-9", result.DocumentNumber);
        Assert.Equal(2, result.ChunkIndex);
        Assert.Equal(500, result.TotalRows);
        Assert.Equal(0, result.InsertedCount);
        Assert.Equal(500, result.FailedCount);
        Assert.Equal(["Failed to write chunk 2: disk full"], result.Errors);
    }

    [Fact]
    public async Task UploadChunkAsync_SuccessfulUpload_ParsesResponseAndDeletesTempFile()
    {
        var chunkFilePath = Path.Combine(_workDir, "chunk_0.csv");
        await File.WriteAllTextAsync(chunkFilePath, "COL1\nrow1\n");

        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ImportResultResponse
            {
                TotalRows = 500,
                InsertedCount = 495,
                FailedCount = 5,
                Errors = [new ImportRowErrorResponse { RowNumber = 10, ErrorMessage = "bad guid" }],
            }),
        });
        var cleaner = new RecordingTempFileCleaner();
        var client = BuildClient(handler, cleaner);

        var chunk = new ChunkDescriptor
        {
            DocumentNumber = "DOC-1",
            ChunkIndex = 0,
            ChunkFilePath = chunkFilePath,
            RowCount = 500,
        };

        var result = await client.UploadChunkAsync(chunk, "http://fake-endpoint.test/import");

        Assert.Equal(1, handler.CallCount);
        Assert.Equal(500, result.TotalRows);
        Assert.Equal(495, result.InsertedCount);
        Assert.Equal(5, result.FailedCount);
        Assert.Equal(["Chunk 0 row 10: bad guid"], result.Errors);
        Assert.Equal([chunkFilePath], cleaner.DeletedPaths);
    }

    [Fact]
    public async Task UploadChunkAsync_NonSuccessStatusCode_ThrowsButStillDeletesTempFile()
    {
        var chunkFilePath = Path.Combine(_workDir, "chunk_1.csv");
        await File.WriteAllTextAsync(chunkFilePath, "COL1\nrow1\n");

        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var cleaner = new RecordingTempFileCleaner();
        var client = BuildClient(handler, cleaner);

        var chunk = new ChunkDescriptor
        {
            DocumentNumber = "DOC-2",
            ChunkIndex = 1,
            ChunkFilePath = chunkFilePath,
            RowCount = 500,
        };

        await Assert.ThrowsAsync<HttpRequestException>(
            () => client.UploadChunkAsync(chunk, "http://fake-endpoint.test/import"));

        Assert.Equal([chunkFilePath], cleaner.DeletedPaths);
    }
}
