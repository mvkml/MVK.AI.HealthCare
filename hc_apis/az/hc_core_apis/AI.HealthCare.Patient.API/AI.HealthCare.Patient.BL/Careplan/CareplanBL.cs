using AI.HealthCare.Patient.Models.Careplan;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class CareplanBL : ICareplanBL
{
    private const int ImportBatchSize = 500;

    private readonly ICareplanRepository _careplanRepository;
    private readonly ICareplanBLMapper _mapper;

    public CareplanBL(ICareplanRepository careplanRepository, ICareplanBLMapper mapper)
    {
        _careplanRepository = careplanRepository;
        _mapper = mapper;
    }

    public async Task<CareplansModel> Create(CareplansModel careplansModel)
    {
        careplansModel.CareplanItem = _mapper.ToItem(careplansModel.CareplanRequest);
        careplansModel.CareplanItem.Id = Guid.NewGuid();

        var savedItem = await _careplanRepository.Create(careplansModel.CareplanItem);
        careplansModel.CareplanItem = savedItem;

        careplansModel.CareplanResponse = _mapper.ToResponse(savedItem);
        careplansModel.IsNotValid = false;
        careplansModel.Message = "Careplan created successfully.";

        return careplansModel;
    }

    public async Task<CareplansModel> GetById(CareplansModel careplansModel)
    {
        var item = await _careplanRepository.GetById(careplansModel.CareplanItem.Id);
        if (item is null)
        {
            careplansModel.IsNotValid = true;
            careplansModel.Message = "Careplan not found.";
            return careplansModel;
        }

        careplansModel.CareplanItem = item;
        careplansModel.CareplanResponse = _mapper.ToResponse(item);
        careplansModel.IsNotValid = false;
        careplansModel.Message = "Careplan retrieved successfully.";
        return careplansModel;
    }

    public async Task<CareplansModel> GetAll(CareplansModel careplansModel)
    {
        var items = await _careplanRepository.GetAll();
        careplansModel.CareplanItems = items;
        careplansModel.IsNotValid = false;
        careplansModel.Message = $"{items.Count} careplan(s) retrieved.";
        return careplansModel;
    }

    public async Task<CareplansModel> GetByPatientId(Guid patientId)
    {
        var items = await _careplanRepository.GetByPatientId(patientId);
        return new CareplansModel
        {
            CareplanItems = items,
            IsNotValid = false,
            Message = $"{items.Count} careplan(s) retrieved for patient."
        };
    }

    public async Task<CareplansModel> Update(CareplansModel careplansModel)
    {
        var existing = await _careplanRepository.GetById(careplansModel.CareplanItem.Id);
        if (existing is null)
        {
            careplansModel.IsNotValid = true;
            careplansModel.Message = "Careplan not found.";
            return careplansModel;
        }

        var updatedItem = _mapper.ToItem(careplansModel.CareplanRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _careplanRepository.Update(updatedItem);
        careplansModel.CareplanItem = savedItem!;
        careplansModel.CareplanResponse = _mapper.ToResponse(savedItem!);
        careplansModel.IsNotValid = false;
        careplansModel.Message = "Careplan updated successfully.";
        return careplansModel;
    }

    public async Task<CareplansModel> Delete(CareplansModel careplansModel)
    {
        var deleted = await _careplanRepository.Delete(careplansModel.CareplanItem.Id);
        if (!deleted)
        {
            careplansModel.IsNotValid = true;
            careplansModel.Message = "Careplan not found.";
            return careplansModel;
        }

        careplansModel.IsNotValid = false;
        careplansModel.Message = "Careplan deleted successfully.";
        return careplansModel;
    }

    public async Task<ImportResult> Import(Stream csvStream)
    {
        var result = new ImportResult();
        var batch = new List<CareplanItem>();

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

    private async Task FlushBatch(List<CareplanItem> batch, ImportResult result)
    {
        try
        {
            await _careplanRepository.CreateBatch(batch);
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
