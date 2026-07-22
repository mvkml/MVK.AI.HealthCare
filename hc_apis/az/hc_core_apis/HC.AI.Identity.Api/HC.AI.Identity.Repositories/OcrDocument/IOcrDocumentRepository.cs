using HC.AI.Identity.Models.OcrDocument;

namespace HC.AI.Identity.Repositories.OcrDocument;

/// <summary>
/// Data persistence operations for OCR documents.
/// </summary>
public interface IOcrDocumentRepository
{
    /// <summary>Returns an active document by primary key, or null if not found.</summary>
    Task<OcrDocumentItem?> GetById(int id);

    /// <summary>Returns an active document by its unique DCN, or null if not found.</summary>
    Task<OcrDocumentItem?> GetByDocumentNumber(string documentNumber);

    /// <summary>Returns all active documents ordered by creation date descending.</summary>
    Task<List<OcrDocumentItem>> GetAll();

    /// <summary>Returns all active documents belonging to a batch group number.</summary>
    Task<List<OcrDocumentItem>> GetByGroupNumber(string groupNumber);

    /// <summary>Inserts a new OCR document record and returns the persisted item.</summary>
    Task<OcrDocumentItem> Create(OcrDocumentItem item);

    /// <summary>Updates status and optional Azure fields on an existing document. Returns null if not found.</summary>
    Task<OcrDocumentItem?> UpdateStatus(int id, string status, string? dfInstanceId = null, string? blobUrl = null, long? blobSize = null);

    /// <summary>Soft-deletes a document by setting IsActive = false. Returns true if found and deleted.</summary>
    Task<bool> Delete(int id);
}
