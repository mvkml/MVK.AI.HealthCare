using AI.HR.Models.OcrDocument;
using AI.HR.Repoistories.OcrDocument;
using AI.HR.Common;

namespace AI.HR.BL.OcrDocument;

/// <summary>
/// Business logic for OCR document operations. Generates DCN/GRP numbers
/// and delegates persistence to IOcrDocumentRepository.
/// </summary>
public class OcrDocumentBL : IOcrDocumentBL
{
    private readonly IOcrDocumentRepository _repository;
    private readonly ICommonUtility _commonUtility;

    public OcrDocumentBL(IOcrDocumentRepository repository, ICommonUtility commonUtility)
    {
        _repository = repository;
        _commonUtility = commonUtility;
    }

    public async Task<OcrDocumentsModel> Upload(OcrDocumentsModel model)
    {
        model.OcrDocumentItem.DocumentNumber = _commonUtility.GenerateDcn();
        model.OcrDocumentItem.DocumentType = model.OcrDocumentRequest.DocumentType;
        model.OcrDocumentItem.GroupNumber = model.OcrDocumentRequest.GroupNumber;
        model.OcrDocumentItem.Status = "Pending";
        model.OcrDocumentItem.IsActive = true;
        model.OcrDocumentItem.CreatedDateTime = DateTime.UtcNow;

        // TODO: Upload file stream to Azure Blob Storage — set BlobUrl and BlobSize on the item.
        // TODO: Trigger Azure Durable Function orchestration — set DfInstanceId on the item.

        var saved = await _repository.Create(model.OcrDocumentItem);
        model.OcrDocumentItem = saved;
        model.OcrDocumentResponse = ToResponse(saved);
        model.IsNotValid = false;
        model.Message = "Document uploaded successfully.";
        return model;
    }

    public async Task<OcrDocumentsModel> Register(OcrDocumentsModel model)
    {
        var request = model.RegisterDocumentRequest;
        var item = new OcrDocumentItem
        {
            DocumentNumber = request.DocumentNumber,
            GroupNumber = request.GroupNumber,
            FileName = request.FileName,
            FileExtension = request.FileExtension,
            DocumentType = request.DocumentType,
            BlobUrl = request.BlobUrl,
            BlobSize = request.BlobSize,
            Status = "Pending",
            IsActive = true,
            CreatedDateTime = DateTime.UtcNow,
        };

        var saved = await _repository.Create(item);
        model.OcrDocumentItem = saved;
        model.OcrDocumentResponse = ToResponse(saved);
        model.IsNotValid = false;
        model.Message = "Document registered successfully.";
        return model;
    }

    public async Task<OcrDocumentsModel> GetById(OcrDocumentsModel model)
    {
        var item = await _repository.GetById(model.OcrDocumentItem.Id);
        if (item is null)
        {
            model.IsNotValid = true;
            model.Message = "Document not found.";
            return model;
        }
        model.OcrDocumentItem = item;
        model.OcrDocumentResponse = ToResponse(item);
        model.IsNotValid = false;
        model.Message = "Document retrieved successfully.";
        return model;
    }

    public async Task<OcrDocumentsModel> GetAll(OcrDocumentsModel model)
    {
        var items = await _repository.GetAll();
        model.OcrDocumentItems = items;
        model.IsNotValid = false;
        model.Message = $"{items.Count} document(s) retrieved.";
        return model;
    }

    public async Task<OcrDocumentsModel> GetByGroupNumber(OcrDocumentsModel model)
    {
        var groupNumber = model.OcrDocumentRequest.GroupNumber ?? string.Empty;
        var items = await _repository.GetByGroupNumber(groupNumber);
        model.OcrDocumentItems = items;
        model.IsNotValid = false;
        model.Message = $"{items.Count} document(s) in group.";
        return model;
    }

    public async Task<OcrDocumentsModel> Delete(OcrDocumentsModel model)
    {
        var deleted = await _repository.Delete(model.OcrDocumentItem.Id);
        model.IsNotValid = !deleted;
        model.Message = deleted ? "Document deleted successfully." : "Document not found.";
        return model;
    }

    private static OcrDocumentResponse ToResponse(OcrDocumentItem item) => new()
    {
        Id = item.Id,
        DocumentNumber = item.DocumentNumber,
        GroupNumber = item.GroupNumber,
        FileName = item.FileName,
        FileExtension = item.FileExtension,
        DocumentType = item.DocumentType,
        BlobUrl = item.BlobUrl,
        BlobSize = item.BlobSize,
        DfInstanceId = item.DfInstanceId,
        Status = item.Status,
        CreatedDateTime = item.CreatedDateTime,
        UpdatedDateTime = item.UpdatedDateTime,
    };
}
