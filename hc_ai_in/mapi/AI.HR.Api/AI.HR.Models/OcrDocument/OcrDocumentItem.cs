namespace AI.HR.Models.OcrDocument;

/// <summary>
/// Internal BL carrier — mirrors the OcrDocument entity.
/// Used to move data between Repository and BL without exposing EF entities upward.
/// </summary>
public class OcrDocumentItem
{
    public int Id { get; set; }

    /// <summary>Unique document control number — DCN-YY-MM-DD-HH-mm-ss.</summary>
    public string DocumentNumber { get; set; } = string.Empty;

    /// <summary>Shared group number for batch uploads — GRP-YY-MM-DD-HH-mm-ss.</summary>
    public string? GroupNumber { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>Local or staging path before Blob upload.</summary>
    public string? SourceLocation { get; set; }

    /// <summary>Azure Blob Storage URL after successful upload.</summary>
    public string? BlobUrl { get; set; }

    /// <summary>File size in bytes.</summary>
    public long? BlobSize { get; set; }

    /// <summary>Azure Durable Function orchestration instance ID.</summary>
    public string? DfInstanceId { get; set; }

    public string Status { get; set; } = "Pending";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
}
