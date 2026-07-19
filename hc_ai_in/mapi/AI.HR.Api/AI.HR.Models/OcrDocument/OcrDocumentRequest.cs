namespace AI.HR.Models.OcrDocument;

/// <summary>
/// Inbound payload from the Angular UI when uploading a document for OCR extraction.
/// The actual file bytes are passed separately as IFormFile in the controller.
/// </summary>
public class OcrDocumentRequest
{
    /// <summary>Document type selected by the user (e.g. "PAN Card", "Aadhaar Card").</summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>Optional group number (GRP-YY-MM-DD-HH-mm-ss) shared across a batch/zip upload.</summary>
    public string? GroupNumber { get; set; }
}
