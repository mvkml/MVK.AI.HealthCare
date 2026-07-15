using AI.HealthCare.Patient.Models.Procedure;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ProcedureBL : IProcedureBL
{
    private const int ImportBatchSize = 500;

    private readonly IProcedureRepository _procedureRepository;
    private readonly IProcedureBLMapper _mapper;

    public ProcedureBL(IProcedureRepository procedureRepository, IProcedureBLMapper mapper)
    {
        _procedureRepository = procedureRepository;
        _mapper = mapper;
    }

    public async Task<ProceduresModel> Create(ProceduresModel proceduresModel)
    {
        proceduresModel.ProcedureItem = _mapper.ToItem(proceduresModel.ProcedureRequest);

        var savedItem = await _procedureRepository.Create(proceduresModel.ProcedureItem);
        proceduresModel.ProcedureItem = savedItem;

        proceduresModel.ProcedureResponse = _mapper.ToResponse(savedItem);
        proceduresModel.IsNotValid = false;
        proceduresModel.Message = "Procedure created successfully.";

        return proceduresModel;
    }

    public async Task<ProceduresModel> GetById(ProceduresModel proceduresModel)
    {
        var item = await _procedureRepository.GetById(proceduresModel.ProcedureItem.Id);
        if (item is null)
        {
            proceduresModel.IsNotValid = true;
            proceduresModel.Message = "Procedure not found.";
            return proceduresModel;
        }

        proceduresModel.ProcedureItem = item;
        proceduresModel.ProcedureResponse = _mapper.ToResponse(item);
        proceduresModel.IsNotValid = false;
        proceduresModel.Message = "Procedure retrieved successfully.";
        return proceduresModel;
    }

    public async Task<ProceduresModel> GetAll(ProceduresModel proceduresModel)
    {
        var items = await _procedureRepository.GetAll();
        proceduresModel.ProcedureItems = items;
        proceduresModel.IsNotValid = false;
        proceduresModel.Message = $"{items.Count} procedure(s) retrieved.";
        return proceduresModel;
    }

    public async Task<ProceduresModel> GetByPatientId(Guid patientId)
    {
        var items = await _procedureRepository.GetByPatientId(patientId);
        return new ProceduresModel
        {
            ProcedureItems = items,
            IsNotValid = false,
            Message = $"{items.Count} procedure(s) retrieved for patient."
        };
    }

    public async Task<ProceduresModel> Update(ProceduresModel proceduresModel)
    {
        var existing = await _procedureRepository.GetById(proceduresModel.ProcedureItem.Id);
        if (existing is null)
        {
            proceduresModel.IsNotValid = true;
            proceduresModel.Message = "Procedure not found.";
            return proceduresModel;
        }

        var updatedItem = _mapper.ToItem(proceduresModel.ProcedureRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _procedureRepository.Update(updatedItem);
        proceduresModel.ProcedureItem = savedItem!;
        proceduresModel.ProcedureResponse = _mapper.ToResponse(savedItem!);
        proceduresModel.IsNotValid = false;
        proceduresModel.Message = "Procedure updated successfully.";
        return proceduresModel;
    }

    public async Task<ProceduresModel> Delete(ProceduresModel proceduresModel)
    {
        var deleted = await _procedureRepository.Delete(proceduresModel.ProcedureItem.Id);
        if (!deleted)
        {
            proceduresModel.IsNotValid = true;
            proceduresModel.Message = "Procedure not found.";
            return proceduresModel;
        }

        proceduresModel.IsNotValid = false;
        proceduresModel.Message = "Procedure deleted successfully.";
        return proceduresModel;
    }

    public async Task<ImportResult> Import(Stream csvStream)
    {
        var result = new ImportResult();
        var batch = new List<ProcedureItem>();

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

    private async Task FlushBatch(List<ProcedureItem> batch, ImportResult result)
    {
        try
        {
            await _procedureRepository.CreateBatch(batch);
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
