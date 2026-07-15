using AI.HealthCare.Patient.Models.ImagingStudy;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ImagingStudyBL : IImagingStudyBL
{
    private const int ImportBatchSize = 500;

    private readonly IImagingStudyRepository _imagingStudyRepository;
    private readonly IImagingStudyBLMapper _mapper;

    public ImagingStudyBL(IImagingStudyRepository imagingStudyRepository, IImagingStudyBLMapper mapper)
    {
        _imagingStudyRepository = imagingStudyRepository;
        _mapper = mapper;
    }

    public async Task<ImagingStudiesModel> Create(ImagingStudiesModel imagingStudiesModel)
    {
        imagingStudiesModel.ImagingStudyItem = _mapper.ToItem(imagingStudiesModel.ImagingStudyRequest);
        imagingStudiesModel.ImagingStudyItem.Id = Guid.NewGuid();

        var savedItem = await _imagingStudyRepository.Create(imagingStudiesModel.ImagingStudyItem);
        imagingStudiesModel.ImagingStudyItem = savedItem;

        imagingStudiesModel.ImagingStudyResponse = _mapper.ToResponse(savedItem);
        imagingStudiesModel.IsNotValid = false;
        imagingStudiesModel.Message = "ImagingStudy created successfully.";

        return imagingStudiesModel;
    }

    public async Task<ImagingStudiesModel> GetById(ImagingStudiesModel imagingStudiesModel)
    {
        var item = await _imagingStudyRepository.GetById(imagingStudiesModel.ImagingStudyItem.Id);
        if (item is null)
        {
            imagingStudiesModel.IsNotValid = true;
            imagingStudiesModel.Message = "ImagingStudy not found.";
            return imagingStudiesModel;
        }

        imagingStudiesModel.ImagingStudyItem = item;
        imagingStudiesModel.ImagingStudyResponse = _mapper.ToResponse(item);
        imagingStudiesModel.IsNotValid = false;
        imagingStudiesModel.Message = "ImagingStudy retrieved successfully.";
        return imagingStudiesModel;
    }

    public async Task<ImagingStudiesModel> GetAll(ImagingStudiesModel imagingStudiesModel)
    {
        var items = await _imagingStudyRepository.GetAll();
        imagingStudiesModel.ImagingStudyItems = items;
        imagingStudiesModel.IsNotValid = false;
        imagingStudiesModel.Message = $"{items.Count} imaging study(ies) retrieved.";
        return imagingStudiesModel;
    }

    public async Task<ImagingStudiesModel> GetByPatientId(Guid patientId)
    {
        var items = await _imagingStudyRepository.GetByPatientId(patientId);
        return new ImagingStudiesModel
        {
            ImagingStudyItems = items,
            IsNotValid = false,
            Message = $"{items.Count} imaging study(ies) retrieved for patient."
        };
    }

    public async Task<ImagingStudiesModel> Update(ImagingStudiesModel imagingStudiesModel)
    {
        var existing = await _imagingStudyRepository.GetById(imagingStudiesModel.ImagingStudyItem.Id);
        if (existing is null)
        {
            imagingStudiesModel.IsNotValid = true;
            imagingStudiesModel.Message = "ImagingStudy not found.";
            return imagingStudiesModel;
        }

        var updatedItem = _mapper.ToItem(imagingStudiesModel.ImagingStudyRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _imagingStudyRepository.Update(updatedItem);
        imagingStudiesModel.ImagingStudyItem = savedItem!;
        imagingStudiesModel.ImagingStudyResponse = _mapper.ToResponse(savedItem!);
        imagingStudiesModel.IsNotValid = false;
        imagingStudiesModel.Message = "ImagingStudy updated successfully.";
        return imagingStudiesModel;
    }

    public async Task<ImagingStudiesModel> Delete(ImagingStudiesModel imagingStudiesModel)
    {
        var deleted = await _imagingStudyRepository.Delete(imagingStudiesModel.ImagingStudyItem.Id);
        if (!deleted)
        {
            imagingStudiesModel.IsNotValid = true;
            imagingStudiesModel.Message = "ImagingStudy not found.";
            return imagingStudiesModel;
        }

        imagingStudiesModel.IsNotValid = false;
        imagingStudiesModel.Message = "ImagingStudy deleted successfully.";
        return imagingStudiesModel;
    }

    public async Task<ImportResult> Import(Stream csvStream) =>
        await RunImport(csvStream, FlushBatch);

    public async Task<ImportResult> ImportUpsert(Stream csvStream) =>
        await RunImport(csvStream, FlushUpsertBatch);

    private async Task<ImportResult> RunImport(Stream csvStream, Func<List<ImagingStudyItem>, ImportResult, Task> flush)
    {
        var result = new ImportResult();
        var batch = new List<ImagingStudyItem>();

        using var reader = new StreamReader(csvStream);
        await reader.ReadLineAsync(); // skip header row

        var rowNumber = 1;
        while (!reader.EndOfStream)
        {
            rowNumber++;
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            result.TotalRows++;

            try
            {
                batch.Add(_mapper.ToModel(line.Split(',')));
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.Errors.Add(new ImportRowError { RowNumber = rowNumber, ErrorMessage = ex.Message });
                continue;
            }

            if (batch.Count >= ImportBatchSize)
                await flush(batch, result);
        }

        if (batch.Count > 0)
            await flush(batch, result);

        return result;
    }

    private async Task FlushBatch(List<ImagingStudyItem> batch, ImportResult result)
    {
        try
        {
            await _imagingStudyRepository.CreateBatch(batch);
            result.InsertedCount += batch.Count;
        }
        catch (Exception ex)
        {
            result.FailedCount += batch.Count;
            result.Errors.Add(new ImportRowError { RowNumber = -1, ErrorMessage = $"Batch insert failed: {ex.Message}" });
        }
        finally
        {
            batch.Clear();
        }
    }

    private async Task FlushUpsertBatch(List<ImagingStudyItem> batch, ImportResult result)
    {
        try
        {
            await _imagingStudyRepository.UpsertBatch(batch);
            result.InsertedCount += batch.Count;
        }
        catch (Exception ex)
        {
            result.FailedCount += batch.Count;
            result.Errors.Add(new ImportRowError { RowNumber = -1, ErrorMessage = $"Batch upsert failed: {ex.Message}" });
        }
        finally
        {
            batch.Clear();
        }
    }
}
