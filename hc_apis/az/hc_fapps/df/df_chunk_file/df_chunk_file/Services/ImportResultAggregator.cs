using df_chunk_file.Models;

namespace df_chunk_file.Services;

/// <summary>Pure in-memory merge of per-chunk results — no I/O, safe to call directly from the orchestrator.</summary>
public class ImportResultAggregator : IImportResultAggregator
{
    public BulkImportSummary Aggregate(string documentNumber, string moduleType, List<ChunkUploadResult> chunkResults)
    {
        return new BulkImportSummary
        {
            DocumentNumber = documentNumber,
            ModuleType = moduleType,
            ChunkCount = chunkResults.Count,
            TotalRows = chunkResults.Sum(c => c.TotalRows),
            InsertedCount = chunkResults.Sum(c => c.InsertedCount),
            FailedCount = chunkResults.Sum(c => c.FailedCount),
            Errors = chunkResults.SelectMany(c => c.Errors).ToList()
        };
    }
}
