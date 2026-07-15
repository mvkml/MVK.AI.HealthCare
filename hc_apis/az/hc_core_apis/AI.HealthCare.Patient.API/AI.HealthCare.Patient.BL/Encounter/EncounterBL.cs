using AI.HealthCare.Patient.Models.Encounter;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class EncounterBL : IEncounterBL
{
    private const int ImportBatchSize = 500;

    private readonly IEncounterRepository _encounterRepository;
    private readonly IEncounterBLMapper _mapper;

    public EncounterBL(IEncounterRepository encounterRepository, IEncounterBLMapper mapper)
    {
        _encounterRepository = encounterRepository;
        _mapper = mapper;
    }

    public async Task<EncountersModel> Create(EncountersModel encountersModel)
    {
        encountersModel.EncounterItem = _mapper.ToItem(encountersModel.EncounterRequest);
        encountersModel.EncounterItem.Id = Guid.NewGuid();

        var savedItem = await _encounterRepository.Create(encountersModel.EncounterItem);
        encountersModel.EncounterItem = savedItem;

        encountersModel.EncounterResponse = _mapper.ToResponse(savedItem);
        encountersModel.IsNotValid = false;
        encountersModel.Message = "Encounter created successfully.";

        return encountersModel;
    }

    public async Task<EncountersModel> GetById(EncountersModel encountersModel)
    {
        var item = await _encounterRepository.GetById(encountersModel.EncounterItem.Id);
        if (item is null)
        {
            encountersModel.IsNotValid = true;
            encountersModel.Message = "Encounter not found.";
            return encountersModel;
        }

        encountersModel.EncounterItem = item;
        encountersModel.EncounterResponse = _mapper.ToResponse(item);
        encountersModel.IsNotValid = false;
        encountersModel.Message = "Encounter retrieved successfully.";
        return encountersModel;
    }

    public async Task<EncountersModel> GetAll(EncountersModel encountersModel)
    {
        var items = await _encounterRepository.GetAll();
        encountersModel.EncounterItems = items;
        encountersModel.IsNotValid = false;
        encountersModel.Message = $"{items.Count} encounter(s) retrieved.";
        return encountersModel;
    }

    public async Task<EncountersModel> GetByPatientId(Guid patientId)
    {
        var items = await _encounterRepository.GetByPatientId(patientId);
        return new EncountersModel
        {
            EncounterItems = items,
            IsNotValid = false,
            Message = $"{items.Count} encounter(s) retrieved for patient."
        };
    }

    public async Task<EncountersModel> Update(EncountersModel encountersModel)
    {
        var existing = await _encounterRepository.GetById(encountersModel.EncounterItem.Id);
        if (existing is null)
        {
            encountersModel.IsNotValid = true;
            encountersModel.Message = "Encounter not found.";
            return encountersModel;
        }

        var updatedItem = _mapper.ToItem(encountersModel.EncounterRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _encounterRepository.Update(updatedItem);
        encountersModel.EncounterItem = savedItem!;
        encountersModel.EncounterResponse = _mapper.ToResponse(savedItem!);
        encountersModel.IsNotValid = false;
        encountersModel.Message = "Encounter updated successfully.";
        return encountersModel;
    }

    public async Task<EncountersModel> Delete(EncountersModel encountersModel)
    {
        var deleted = await _encounterRepository.Delete(encountersModel.EncounterItem.Id);
        if (!deleted)
        {
            encountersModel.IsNotValid = true;
            encountersModel.Message = "Encounter not found.";
            return encountersModel;
        }

        encountersModel.IsNotValid = false;
        encountersModel.Message = "Encounter deleted successfully.";
        return encountersModel;
    }

    public async Task<ImportResult> Import(Stream csvStream) =>
        await RunImport(csvStream, FlushBatch);

    public async Task<ImportResult> ImportUpsert(Stream csvStream) =>
        await RunImport(csvStream, FlushUpsertBatch);

    private async Task<ImportResult> RunImport(Stream csvStream, Func<List<EncounterItem>, ImportResult, Task> flush)
    {
        var result = new ImportResult();
        var batch = new List<EncounterItem>();

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

    private async Task FlushBatch(List<EncounterItem> batch, ImportResult result)
    {
        try
        {
            await _encounterRepository.CreateBatch(batch);
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

    private async Task FlushUpsertBatch(List<EncounterItem> batch, ImportResult result)
    {
        try
        {
            await _encounterRepository.UpsertBatch(batch);
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
