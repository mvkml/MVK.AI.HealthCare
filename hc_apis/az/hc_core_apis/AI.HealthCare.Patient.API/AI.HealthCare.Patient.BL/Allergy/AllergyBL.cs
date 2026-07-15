using AI.HealthCare.Patient.Models.Allergy;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class AllergyBL : IAllergyBL
{
    private const int ImportBatchSize = 500;

    private readonly IAllergyRepository _allergyRepository;
    private readonly IAllergyCsvMapper _csvMapper;

    public AllergyBL(IAllergyRepository allergyRepository, IAllergyCsvMapper csvMapper)
    {
        _allergyRepository = allergyRepository;
        _csvMapper = csvMapper;
    }

    public async Task<AllergiesModel> Create(AllergiesModel allergiesModel)
    {
        allergiesModel.AllergyItem = _csvMapper.ToItem(allergiesModel.AllergyRequest);

        var savedItem = await _allergyRepository.Create(allergiesModel.AllergyItem);
        allergiesModel.AllergyItem = savedItem;

        allergiesModel.AllergyResponse = _csvMapper.ToResponse(savedItem);
        allergiesModel.IsNotValid = false;
        allergiesModel.Message = "Allergy created successfully.";

        return allergiesModel;
    }

    public async Task<AllergiesModel> GetById(AllergiesModel allergiesModel)
    {
        var item = await _allergyRepository.GetById(allergiesModel.AllergyItem.Id);
        if (item is null)
        {
            allergiesModel.IsNotValid = true;
            allergiesModel.Message = "Allergy not found.";
            return allergiesModel;
        }

        allergiesModel.AllergyItem = item;
        allergiesModel.AllergyResponse = _csvMapper.ToResponse(item);
        allergiesModel.IsNotValid = false;
        allergiesModel.Message = "Allergy retrieved successfully.";
        return allergiesModel;
    }

    public async Task<AllergiesModel> GetAll(AllergiesModel allergiesModel)
    {
        var items = await _allergyRepository.GetAll();
        allergiesModel.AllergyItems = items;
        allergiesModel.IsNotValid = false;
        allergiesModel.Message = $"{items.Count} allergy(ies) retrieved.";
        return allergiesModel;
    }

    public async Task<AllergiesModel> GetByPatientId(Guid patientId)
    {
        var items = await _allergyRepository.GetByPatientId(patientId);
        return new AllergiesModel
        {
            AllergyItems = items,
            IsNotValid = false,
            Message = $"{items.Count} allergy(ies) retrieved for patient."
        };
    }

    public async Task<AllergiesModel> Update(AllergiesModel allergiesModel)
    {
        var existing = await _allergyRepository.GetById(allergiesModel.AllergyItem.Id);
        if (existing is null)
        {
            allergiesModel.IsNotValid = true;
            allergiesModel.Message = "Allergy not found.";
            return allergiesModel;
        }

        var updatedItem = _csvMapper.ToItem(allergiesModel.AllergyRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _allergyRepository.Update(updatedItem);
        allergiesModel.AllergyItem = savedItem!;
        allergiesModel.AllergyResponse = _csvMapper.ToResponse(savedItem!);
        allergiesModel.IsNotValid = false;
        allergiesModel.Message = "Allergy updated successfully.";
        return allergiesModel;
    }

    public async Task<AllergiesModel> Delete(AllergiesModel allergiesModel)
    {
        var deleted = await _allergyRepository.Delete(allergiesModel.AllergyItem.Id);
        if (!deleted)
        {
            allergiesModel.IsNotValid = true;
            allergiesModel.Message = "Allergy not found.";
            return allergiesModel;
        }

        allergiesModel.IsNotValid = false;
        allergiesModel.Message = "Allergy deleted successfully.";
        return allergiesModel;
    }

    public async Task<ImportResult> Import(Stream csvStream) =>
        await RunImport(csvStream, FlushBatch);

    public async Task<ImportResult> ImportUpsert(Stream csvStream) =>
        await RunImport(csvStream, FlushUpsertBatch);

    private async Task<ImportResult> RunImport(Stream csvStream, Func<List<AllergyItem>, ImportResult, Task> flush)
    {
        var result = new ImportResult();
        var batch = new List<AllergyItem>();

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
                batch.Add(_csvMapper.ToModel(line.Split(',')));
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

    private async Task FlushBatch(List<AllergyItem> batch, ImportResult result)
    {
        try
        {
            await _allergyRepository.CreateBatch(batch);
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

    private async Task FlushUpsertBatch(List<AllergyItem> batch, ImportResult result)
    {
        try
        {
            await _allergyRepository.UpsertBatch(batch);
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
