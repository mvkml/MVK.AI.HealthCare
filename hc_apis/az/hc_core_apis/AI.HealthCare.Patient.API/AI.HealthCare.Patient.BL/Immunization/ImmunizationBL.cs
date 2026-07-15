using AI.HealthCare.Patient.Models.Immunization;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ImmunizationBL : IImmunizationBL
{
    private const int ImportBatchSize = 500;

    private readonly IImmunizationRepository _immunizationRepository;
    private readonly IImmunizationBLMapper _mapper;

    public ImmunizationBL(IImmunizationRepository immunizationRepository, IImmunizationBLMapper mapper)
    {
        _immunizationRepository = immunizationRepository;
        _mapper = mapper;
    }

    public async Task<ImmunizationsModel> Create(ImmunizationsModel immunizationsModel)
    {
        immunizationsModel.ImmunizationItem = _mapper.ToItem(immunizationsModel.ImmunizationRequest);

        var savedItem = await _immunizationRepository.Create(immunizationsModel.ImmunizationItem);
        immunizationsModel.ImmunizationItem = savedItem;

        immunizationsModel.ImmunizationResponse = _mapper.ToResponse(savedItem);
        immunizationsModel.IsNotValid = false;
        immunizationsModel.Message = "Immunization created successfully.";

        return immunizationsModel;
    }

    public async Task<ImmunizationsModel> GetById(ImmunizationsModel immunizationsModel)
    {
        var item = await _immunizationRepository.GetById(immunizationsModel.ImmunizationItem.Id);
        if (item is null)
        {
            immunizationsModel.IsNotValid = true;
            immunizationsModel.Message = "Immunization not found.";
            return immunizationsModel;
        }

        immunizationsModel.ImmunizationItem = item;
        immunizationsModel.ImmunizationResponse = _mapper.ToResponse(item);
        immunizationsModel.IsNotValid = false;
        immunizationsModel.Message = "Immunization retrieved successfully.";
        return immunizationsModel;
    }

    public async Task<ImmunizationsModel> GetAll(ImmunizationsModel immunizationsModel)
    {
        var items = await _immunizationRepository.GetAll();
        immunizationsModel.ImmunizationItems = items;
        immunizationsModel.IsNotValid = false;
        immunizationsModel.Message = $"{items.Count} immunization(s) retrieved.";
        return immunizationsModel;
    }

    public async Task<ImmunizationsModel> GetByPatientId(Guid patientId)
    {
        var items = await _immunizationRepository.GetByPatientId(patientId);
        return new ImmunizationsModel
        {
            ImmunizationItems = items,
            IsNotValid = false,
            Message = $"{items.Count} immunization(s) retrieved for patient."
        };
    }

    public async Task<ImmunizationsModel> Update(ImmunizationsModel immunizationsModel)
    {
        var existing = await _immunizationRepository.GetById(immunizationsModel.ImmunizationItem.Id);
        if (existing is null)
        {
            immunizationsModel.IsNotValid = true;
            immunizationsModel.Message = "Immunization not found.";
            return immunizationsModel;
        }

        var updatedItem = _mapper.ToItem(immunizationsModel.ImmunizationRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _immunizationRepository.Update(updatedItem);
        immunizationsModel.ImmunizationItem = savedItem!;
        immunizationsModel.ImmunizationResponse = _mapper.ToResponse(savedItem!);
        immunizationsModel.IsNotValid = false;
        immunizationsModel.Message = "Immunization updated successfully.";
        return immunizationsModel;
    }

    public async Task<ImmunizationsModel> Delete(ImmunizationsModel immunizationsModel)
    {
        var deleted = await _immunizationRepository.Delete(immunizationsModel.ImmunizationItem.Id);
        if (!deleted)
        {
            immunizationsModel.IsNotValid = true;
            immunizationsModel.Message = "Immunization not found.";
            return immunizationsModel;
        }

        immunizationsModel.IsNotValid = false;
        immunizationsModel.Message = "Immunization deleted successfully.";
        return immunizationsModel;
    }

    public async Task<ImportResult> Import(Stream csvStream) =>
        await RunImport(csvStream, FlushBatch);

    public async Task<ImportResult> ImportUpsert(Stream csvStream) =>
        await RunImport(csvStream, FlushUpsertBatch);

    private async Task<ImportResult> RunImport(Stream csvStream, Func<List<ImmunizationItem>, ImportResult, Task> flush)
    {
        var result = new ImportResult();
        var batch = new List<ImmunizationItem>();

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

    private async Task FlushBatch(List<ImmunizationItem> batch, ImportResult result)
    {
        try
        {
            await _immunizationRepository.CreateBatch(batch);
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

    private async Task FlushUpsertBatch(List<ImmunizationItem> batch, ImportResult result)
    {
        try
        {
            await _immunizationRepository.UpsertBatch(batch);
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
