using df_chunk_file.Models;

namespace df_chunk_file.Services;

public interface IImportResultAggregator
{
    BulkImportSummary Aggregate(string documentNumber, string moduleType, List<ChunkUploadResult> chunkResults);
}
