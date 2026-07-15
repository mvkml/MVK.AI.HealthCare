using AI.HealthCare.Patient.Models.Supply;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class SupplyBL : ISupplyBL
{
    private const int ImportBatchSize = 500;

    private readonly ISupplyRepository _supplyRepository;
    private readonly ISupplyBLMapper _mapper;

    public SupplyBL(ISupplyRepository supplyRepository, ISupplyBLMapper mapper)
    {
        _supplyRepository = supplyRepository;
        _mapper = mapper;
    }

    public async Task<SuppliesModel> Create(SuppliesModel suppliesModel)
    {
        suppliesModel.SupplyItem = _mapper.ToItem(suppliesModel.SupplyRequest);

        var savedItem = await _supplyRepository.Create(suppliesModel.SupplyItem);
        suppliesModel.SupplyItem = savedItem;

        suppliesModel.SupplyResponse = _mapper.ToResponse(savedItem);
        suppliesModel.IsNotValid = false;
        suppliesModel.Message = "Supply created successfully.";

        return suppliesModel;
    }

    public async Task<SuppliesModel> GetById(SuppliesModel suppliesModel)
    {
        var item = await _supplyRepository.GetById(suppliesModel.SupplyItem.Id);
        if (item is null)
        {
            suppliesModel.IsNotValid = true;
            suppliesModel.Message = "Supply not found.";
            return suppliesModel;
        }

        suppliesModel.SupplyItem = item;
        suppliesModel.SupplyResponse = _mapper.ToResponse(item);
        suppliesModel.IsNotValid = false;
        suppliesModel.Message = "Supply retrieved successfully.";
        return suppliesModel;
    }

    public async Task<SuppliesModel> GetAll(SuppliesModel suppliesModel)
    {
        var items = await _supplyRepository.GetAll();
        suppliesModel.SupplyItems = items;
        suppliesModel.IsNotValid = false;
        suppliesModel.Message = $"{items.Count} supply(ies) retrieved.";
        return suppliesModel;
    }

    public async Task<SuppliesModel> GetByPatientId(Guid patientId)
    {
        var items = await _supplyRepository.GetByPatientId(patientId);
        return new SuppliesModel
        {
            SupplyItems = items,
            IsNotValid = false,
            Message = $"{items.Count} supply(ies) retrieved for patient."
        };
    }

    public async Task<SuppliesModel> Update(SuppliesModel suppliesModel)
    {
        var existing = await _supplyRepository.GetById(suppliesModel.SupplyItem.Id);
        if (existing is null)
        {
            suppliesModel.IsNotValid = true;
            suppliesModel.Message = "Supply not found.";
            return suppliesModel;
        }

        var updatedItem = _mapper.ToItem(suppliesModel.SupplyRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _supplyRepository.Update(updatedItem);
        suppliesModel.SupplyItem = savedItem!;
        suppliesModel.SupplyResponse = _mapper.ToResponse(savedItem!);
        suppliesModel.IsNotValid = false;
        suppliesModel.Message = "Supply updated successfully.";
        return suppliesModel;
    }

    public async Task<SuppliesModel> Delete(SuppliesModel suppliesModel)
    {
        var deleted = await _supplyRepository.Delete(suppliesModel.SupplyItem.Id);
        if (!deleted)
        {
            suppliesModel.IsNotValid = true;
            suppliesModel.Message = "Supply not found.";
            return suppliesModel;
        }

        suppliesModel.IsNotValid = false;
        suppliesModel.Message = "Supply deleted successfully.";
        return suppliesModel;
    }

    public async Task<ImportResult> Import(Stream csvStream)
    {
        var result = new ImportResult();
        var batch = new List<SupplyItem>();

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
                await FlushBatch(batch, result);
        }

        if (batch.Count > 0)
            await FlushBatch(batch, result);

        return result;
    }

    private async Task FlushBatch(List<SupplyItem> batch, ImportResult result)
    {
        try
        {
            await _supplyRepository.CreateBatch(batch);
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
}
