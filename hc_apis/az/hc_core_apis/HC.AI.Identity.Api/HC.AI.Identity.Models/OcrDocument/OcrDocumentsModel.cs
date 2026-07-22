using HC.AI.Identity.Models;

namespace HC.AI.Identity.Models.OcrDocument;

/// <summary>
/// Carrier model passed between Controller, Validation, and Business Layer for OCR document operations.
/// Wraps request + single item + multiple items + response in one envelope.
/// </summary>
public class OcrDocumentsModel : BaseModel
{
    /// <summary>Incoming request data set by the Controller.</summary>
    public OcrDocumentRequest OcrDocumentRequest { get; set; } = new();

    /// <summary>Incoming request from fa_upload_doc — DCN/GRP already generated, file already in Blob Storage.</summary>
    public RegisterDocumentRequest RegisterDocumentRequest { get; set; } = new();

    /// <summary>Use when the operation concerns a single document.</summary>
    public OcrDocumentItem OcrDocumentItem { get; set; } = new();

    /// <summary>Use when the response contains multiple documents (e.g. batch upload, list query).</summary>
    public List<OcrDocumentItem> OcrDocumentItems { get; set; } = new();

    /// <summary>Outgoing response built by the Business Layer, returned to the Controller.</summary>
    public OcrDocumentResponse OcrDocumentResponse { get; set; } = new();
}
