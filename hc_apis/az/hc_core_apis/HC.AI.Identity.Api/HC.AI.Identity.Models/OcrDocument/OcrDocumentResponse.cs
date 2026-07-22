using HC.AI.Identity.Models;

namespace HC.AI.Identity.Models.OcrDocument;

/// <summary>
/// Outgoing response returned to the Angular UI after a document upload or status query.
/// Inherits IsNotValid and Message from BaseModel.
/// </summary>
public class OcrDocumentResponse : BaseModel
{
    public int Id { get; set; }

    /// <summary>Unique document control number — DCN-YY-MM-DD-HH-mm-ss.</summary>
    public string DocumentNumber { get; set; } = string.Empty;

    /// <summary>Shared group number for batch uploads — GRP-YY-MM-DD-HH-mm-ss.</summary>
    public string? GroupNumber { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>Azure Blob Storage URL — available after upload completes.</summary>
    public string? BlobUrl { get; set; }

    /// <summary>File size in bytes.</summary>
    public long? BlobSize { get; set; }

    /// <summary>Azure Durable Function orchestration instance ID.</summary>
    public string? DfInstanceId { get; set; }

    public string Status { get; set; } = "Pending";
    public DateTime CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
}
