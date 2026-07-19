using AI.HR.Models.OcrDocument;

namespace AI.HR.BL.OcrDocument;

/// <summary>
/// Business logic for OCR document operations.
/// </summary>
public interface IOcrDocumentBL
{
    /// <summary>Registers an uploaded document: generates DCN, persists record, returns response.</summary>
    Task<OcrDocumentsModel> Upload(OcrDocumentsModel model);

    /// <summary>
    /// Persists a document record already built by fa_upload_doc (DCN/GRP already
    /// generated, file already in Blob Storage). Does not generate numbers itself.
    /// </summary>
    Task<OcrDocumentsModel> Register(OcrDocumentsModel model);

    /// <summary>Returns a single document by its Id.</summary>
    Task<OcrDocumentsModel> GetById(OcrDocumentsModel model);

    /// <summary>Returns all active documents.</summary>
    Task<OcrDocumentsModel> GetAll(OcrDocumentsModel model);

    /// <summary>Returns all documents belonging to a batch group number.</summary>
    Task<OcrDocumentsModel> GetByGroupNumber(OcrDocumentsModel model);

    /// <summary>Soft-deletes a document by Id.</summary>
    Task<OcrDocumentsModel> Delete(OcrDocumentsModel model);
}
