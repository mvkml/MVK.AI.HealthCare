using AI.HealthCare.Patient.Models.Medication;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class MedicationBL : IMedicationBL
{
    private const int ImportBatchSize = 500;

    private readonly IMedicationRepository _medicationRepository;
    private readonly IMedicationBLMapper _mapper;

    public MedicationBL(IMedicationRepository medicationRepository, IMedicationBLMapper mapper)
    {
        _medicationRepository = medicationRepository;
        _mapper = mapper;
    }

    public async Task<MedicationsModel> Create(MedicationsModel medicationsModel)
    {
        medicationsModel.MedicationItem = _mapper.ToItem(medicationsModel.MedicationRequest);

        var savedItem = await _medicationRepository.Create(medicationsModel.MedicationItem);
        medicationsModel.MedicationItem = savedItem;

        medicationsModel.MedicationResponse = _mapper.ToResponse(savedItem);
        medicationsModel.IsNotValid = false;
        medicationsModel.Message = "Medication created successfully.";

        return medicationsModel;
    }

    public async Task<MedicationsModel> GetById(MedicationsModel medicationsModel)
    {
        var item = await _medicationRepository.GetById(medicationsModel.MedicationItem.Id);
        if (item is null)
        {
            medicationsModel.IsNotValid = true;
            medicationsModel.Message = "Medication not found.";
            return medicationsModel;
        }

        medicationsModel.MedicationItem = item;
        medicationsModel.MedicationResponse = _mapper.ToResponse(item);
        medicationsModel.IsNotValid = false;
        medicationsModel.Message = "Medication retrieved successfully.";
        return medicationsModel;
    }

    public async Task<MedicationsModel> GetAll(MedicationsModel medicationsModel)
    {
        var items = await _medicationRepository.GetAll();
        medicationsModel.MedicationItems = items;
        medicationsModel.IsNotValid = false;
        medicationsModel.Message = $"{items.Count} medication(s) retrieved.";
        return medicationsModel;
    }

    public async Task<MedicationsModel> GetByPatientId(Guid patientId)
    {
        var items = await _medicationRepository.GetByPatientId(patientId);
        return new MedicationsModel
        {
            MedicationItems = items,
            IsNotValid = false,
            Message = $"{items.Count} medication(s) retrieved for patient."
        };
    }

    public async Task<MedicationsModel> Update(MedicationsModel medicationsModel)
    {
        var existing = await _medicationRepository.GetById(medicationsModel.MedicationItem.Id);
        if (existing is null)
        {
            medicationsModel.IsNotValid = true;
            medicationsModel.Message = "Medication not found.";
            return medicationsModel;
        }

        var updatedItem = _mapper.ToItem(medicationsModel.MedicationRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _medicationRepository.Update(updatedItem);
        medicationsModel.MedicationItem = savedItem!;
        medicationsModel.MedicationResponse = _mapper.ToResponse(savedItem!);
        medicationsModel.IsNotValid = false;
        medicationsModel.Message = "Medication updated successfully.";
        return medicationsModel;
    }

    public async Task<MedicationsModel> Delete(MedicationsModel medicationsModel)
    {
        var deleted = await _medicationRepository.Delete(medicationsModel.MedicationItem.Id);
        if (!deleted)
        {
            medicationsModel.IsNotValid = true;
            medicationsModel.Message = "Medication not found.";
            return medicationsModel;
        }

        medicationsModel.IsNotValid = false;
        medicationsModel.Message = "Medication deleted successfully.";
        return medicationsModel;
    }

    public async Task<ImportResult> Import(Stream csvStream) =>
        await RunImport(csvStream, FlushBatch);

    public async Task<ImportResult> ImportUpsert(Stream csvStream) =>
        await RunImport(csvStream, FlushUpsertBatch);

    private async Task<ImportResult> RunImport(Stream csvStream, Func<List<MedicationItem>, ImportResult, Task> flush)
    {
        var result = new ImportResult();
        var batch = new List<MedicationItem>();

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

    private async Task FlushBatch(List<MedicationItem> batch, ImportResult result)
    {
        try
        {
            await _medicationRepository.CreateBatch(batch);
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

    private async Task FlushUpsertBatch(List<MedicationItem> batch, ImportResult result)
    {
        try
        {
            await _medicationRepository.UpsertBatch(batch);
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
