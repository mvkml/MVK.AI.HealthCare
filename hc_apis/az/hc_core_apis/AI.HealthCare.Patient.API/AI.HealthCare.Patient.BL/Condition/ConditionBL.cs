using AI.HealthCare.Patient.Models.Condition;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ConditionBL : IConditionBL
{
    private const int ImportBatchSize = 500;

    private readonly IConditionRepository _conditionRepository;
    private readonly IConditionBLMapper _mapper;

    public ConditionBL(IConditionRepository conditionRepository, IConditionBLMapper mapper)
    {
        _conditionRepository = conditionRepository;
        _mapper = mapper;
    }

    public async Task<ConditionsModel> Create(ConditionsModel conditionsModel)
    {
        conditionsModel.ConditionItem = _mapper.ToItem(conditionsModel.ConditionRequest);

        var savedItem = await _conditionRepository.Create(conditionsModel.ConditionItem);
        conditionsModel.ConditionItem = savedItem;

        conditionsModel.ConditionResponse = _mapper.ToResponse(savedItem);
        conditionsModel.IsNotValid = false;
        conditionsModel.Message = "Condition created successfully.";

        return conditionsModel;
    }

    public async Task<ConditionsModel> GetById(ConditionsModel conditionsModel)
    {
        var item = await _conditionRepository.GetById(conditionsModel.ConditionItem.Id);
        if (item is null)
        {
            conditionsModel.IsNotValid = true;
            conditionsModel.Message = "Condition not found.";
            return conditionsModel;
        }

        conditionsModel.ConditionItem = item;
        conditionsModel.ConditionResponse = _mapper.ToResponse(item);
        conditionsModel.IsNotValid = false;
        conditionsModel.Message = "Condition retrieved successfully.";
        return conditionsModel;
    }

    public async Task<ConditionsModel> GetAll(ConditionsModel conditionsModel)
    {
        var items = await _conditionRepository.GetAll();
        conditionsModel.ConditionItems = items;
        conditionsModel.IsNotValid = false;
        conditionsModel.Message = $"{items.Count} condition(s) retrieved.";
        return conditionsModel;
    }

    public async Task<ConditionsModel> GetByPatientId(Guid patientId)
    {
        var items = await _conditionRepository.GetByPatientId(patientId);
        return new ConditionsModel
        {
            ConditionItems = items,
            IsNotValid = false,
            Message = $"{items.Count} condition(s) retrieved for patient."
        };
    }

    public async Task<ConditionsModel> Update(ConditionsModel conditionsModel)
    {
        var existing = await _conditionRepository.GetById(conditionsModel.ConditionItem.Id);
        if (existing is null)
        {
            conditionsModel.IsNotValid = true;
            conditionsModel.Message = "Condition not found.";
            return conditionsModel;
        }

        var updatedItem = _mapper.ToItem(conditionsModel.ConditionRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _conditionRepository.Update(updatedItem);
        conditionsModel.ConditionItem = savedItem!;
        conditionsModel.ConditionResponse = _mapper.ToResponse(savedItem!);
        conditionsModel.IsNotValid = false;
        conditionsModel.Message = "Condition updated successfully.";
        return conditionsModel;
    }

    public async Task<ConditionsModel> Delete(ConditionsModel conditionsModel)
    {
        var deleted = await _conditionRepository.Delete(conditionsModel.ConditionItem.Id);
        if (!deleted)
        {
            conditionsModel.IsNotValid = true;
            conditionsModel.Message = "Condition not found.";
            return conditionsModel;
        }

        conditionsModel.IsNotValid = false;
        conditionsModel.Message = "Condition deleted successfully.";
        return conditionsModel;
    }

    public async Task<ImportResult> Import(Stream csvStream) =>
        await RunImport(csvStream, FlushBatch);

    public async Task<ImportResult> ImportUpsert(Stream csvStream) =>
        await RunImport(csvStream, FlushUpsertBatch);

    private async Task<ImportResult> RunImport(Stream csvStream, Func<List<ConditionItem>, ImportResult, Task> flush)
    {
        var result = new ImportResult();
        var batch = new List<ConditionItem>();

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

    private async Task FlushBatch(List<ConditionItem> batch, ImportResult result)
    {
        try
        {
            await _conditionRepository.CreateBatch(batch);
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

    private async Task FlushUpsertBatch(List<ConditionItem> batch, ImportResult result)
    {
        try
        {
            await _conditionRepository.UpsertBatch(batch);
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
