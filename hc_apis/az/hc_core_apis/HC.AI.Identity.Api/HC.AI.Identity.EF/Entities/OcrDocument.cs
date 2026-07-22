namespace HC.AI.Identity.EF.Entities;

public class OcrDocument
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string? GroupNumber { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string? SourceLocation { get; set; }
    public string? BlobUrl { get; set; }
    public long? BlobSize { get; set; }
    public string? DfInstanceId { get; set; }
    public string Status { get; set; } = "Pending";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDateTime { get; set; }
}
