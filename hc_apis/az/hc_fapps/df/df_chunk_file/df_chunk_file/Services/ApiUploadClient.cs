using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using df_chunk_file.Models;
using df_chunk_file.Utilities;

namespace df_chunk_file.Services;

/// <summary>Uploads one chunk file to the target REST API endpoint via multipart/form-data, matching how a
/// human would upload a smaller CSV through Swagger. Never touches SQL Server directly.</summary>
public class ApiUploadClient : IApiUploadClient
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _httpClient;
    private readonly ITempFileCleaner _tempFileCleaner;

    public ApiUploadClient(HttpClient httpClient, ITempFileCleaner tempFileCleaner)
    {
        _httpClient = httpClient;
        _tempFileCleaner = tempFileCleaner;
    }

    public async Task<ChunkUploadResult> UploadChunkAsync(ChunkDescriptor chunk, string apiEndpoint)
    {
        // Chunk was already marked invalid during splitting (e.g. failed to write to disk) — skip the
        // HTTP call entirely; only this chunk is affected, the rest of the file still gets processed.
        if (chunk.IsNotValid)
        {
            return new ChunkUploadResult
            {
                DocumentNumber = chunk.DocumentNumber,
                ChunkIndex = chunk.ChunkIndex,
                TotalRows = chunk.RowCount,
                InsertedCount = 0,
                FailedCount = chunk.RowCount,
                Errors = [chunk.Message]
            };
        }

        try
        {
            ImportResultResponse importResult;

            using (var content = new MultipartFormDataContent())
            await using (var fileStream = File.OpenRead(chunk.ChunkFilePath))
            {
                using var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                content.Add(streamContent, "file", Path.GetFileName(chunk.ChunkFilePath));

                var response = await _httpClient.PostAsync(apiEndpoint, content);
                response.EnsureSuccessStatusCode();

                importResult = await response.Content.ReadFromJsonAsync<ImportResultResponse>(JsonOptions)
                    ?? new ImportResultResponse();
            }

            return new ChunkUploadResult
            {
                DocumentNumber = chunk.DocumentNumber,
                ChunkIndex = chunk.ChunkIndex,
                TotalRows = importResult.TotalRows,
                InsertedCount = importResult.InsertedCount,
                FailedCount = importResult.FailedCount,
                Errors = importResult.Errors
                    .Select(e => $"Chunk {chunk.ChunkIndex} row {e.RowNumber}: {e.ErrorMessage}")
                    .ToList()
            };
        }
        finally
        {
            _tempFileCleaner.DeleteFile(chunk.ChunkFilePath);
        }
    }
}
