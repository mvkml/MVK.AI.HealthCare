using HC.AI.Identity.EF.DBContexts;
using HC.AI.Identity.Models.OcrDocument;
using EfOcrDoc = HC.AI.Identity.EF.Entities.OcrDocument;
using Microsoft.EntityFrameworkCore;

namespace HC.AI.Identity.Repositories.OcrDocument;

/// <summary>
/// Data persistence operations for OCR documents, backed by AiHrDbContext.
/// </summary>
public class OcrDocumentRepository : IOcrDocumentRepository
{
    private readonly AiHrDbContext _context;

    public OcrDocumentRepository(AiHrDbContext context)
    {
        _context = context;
    }

    public async Task<OcrDocumentItem?> GetById(int id)
    {
        var entity = await _context.OcrDocuments.FirstOrDefaultAsync(o => o.Id == id && o.IsActive);
        return entity is null ? null : ToModel(entity);
    }

    public async Task<OcrDocumentItem?> GetByDocumentNumber(string documentNumber)
    {
        var entity = await _context.OcrDocuments.FirstOrDefaultAsync(o => o.DocumentNumber == documentNumber && o.IsActive);
        return entity is null ? null : ToModel(entity);
    }

    public async Task<List<OcrDocumentItem>> GetAll()
    {
        var entities = await _context.OcrDocuments
            .Where(o => o.IsActive)
            .OrderByDescending(o => o.CreatedDateTime)
            .ToListAsync();
        return entities.Select(ToModel).ToList();
    }

    public async Task<List<OcrDocumentItem>> GetByGroupNumber(string groupNumber)
    {
        var entities = await _context.OcrDocuments
            .Where(o => o.GroupNumber == groupNumber && o.IsActive)
            .OrderByDescending(o => o.CreatedDateTime)
            .ToListAsync();
        return entities.Select(ToModel).ToList();
    }

    public async Task<OcrDocumentItem> Create(OcrDocumentItem item)
    {
        var entity = ToEntity(item);
        _context.OcrDocuments.Add(entity);
        await _context.SaveChangesAsync();
        return ToModel(entity);
    }

    public async Task<OcrDocumentItem?> UpdateStatus(int id, string status, string? dfInstanceId = null, string? blobUrl = null, long? blobSize = null)
    {
        var entity = await _context.OcrDocuments.FirstOrDefaultAsync(o => o.Id == id);
        if (entity is null) return null;

        entity.Status = status;
        entity.UpdatedDateTime = DateTime.UtcNow;
        if (dfInstanceId is not null) entity.DfInstanceId = dfInstanceId;
        if (blobUrl is not null) entity.BlobUrl = blobUrl;
        if (blobSize is not null) entity.BlobSize = blobSize;

        await _context.SaveChangesAsync();
        return ToModel(entity);
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _context.OcrDocuments.FirstOrDefaultAsync(o => o.Id == id);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.UpdatedDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static EfOcrDoc ToEntity(OcrDocumentItem item) => new()
    {
        DocumentNumber = item.DocumentNumber,
        GroupNumber = item.GroupNumber,
        FileName = item.FileName,
        FileExtension = item.FileExtension,
        DocumentType = item.DocumentType,
        SourceLocation = item.SourceLocation,
        BlobUrl = item.BlobUrl,
        BlobSize = item.BlobSize,
        DfInstanceId = item.DfInstanceId,
        Status = item.Status,
        IsActive = item.IsActive,
        CreatedDateTime = item.CreatedDateTime,
        UpdatedDateTime = item.UpdatedDateTime,
    };

    private static OcrDocumentItem ToModel(EfOcrDoc entity) => new()
    {
        Id = entity.Id,
        DocumentNumber = entity.DocumentNumber,
        GroupNumber = entity.GroupNumber,
        FileName = entity.FileName,
        FileExtension = entity.FileExtension,
        DocumentType = entity.DocumentType,
        SourceLocation = entity.SourceLocation,
        BlobUrl = entity.BlobUrl,
        BlobSize = entity.BlobSize,
        DfInstanceId = entity.DfInstanceId,
        Status = entity.Status,
        IsActive = entity.IsActive,
        CreatedDateTime = entity.CreatedDateTime,
        UpdatedDateTime = entity.UpdatedDateTime,
    };
}
