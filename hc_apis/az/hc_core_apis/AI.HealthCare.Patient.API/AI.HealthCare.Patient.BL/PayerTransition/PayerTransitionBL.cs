using AI.HealthCare.Patient.Models.PayerTransition;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class PayerTransitionBL : IPayerTransitionBL
{
    private const int ImportBatchSize = 500;

    private readonly IPayerTransitionRepository _payerTransitionRepository;
    private readonly IPayerTransitionBLMapper _mapper;

    public PayerTransitionBL(IPayerTransitionRepository payerTransitionRepository, IPayerTransitionBLMapper mapper)
    {
        _payerTransitionRepository = payerTransitionRepository;
        _mapper = mapper;
    }

    public async Task<PayerTransitionsModel> Create(PayerTransitionsModel payerTransitionsModel)
    {
        payerTransitionsModel.PayerTransitionItem = _mapper.ToItem(payerTransitionsModel.PayerTransitionRequest);

        var savedItem = await _payerTransitionRepository.Create(payerTransitionsModel.PayerTransitionItem);
        payerTransitionsModel.PayerTransitionItem = savedItem;

        payerTransitionsModel.PayerTransitionResponse = _mapper.ToResponse(savedItem);
        payerTransitionsModel.IsNotValid = false;
        payerTransitionsModel.Message = "PayerTransition created successfully.";

        return payerTransitionsModel;
    }

    public async Task<PayerTransitionsModel> GetById(PayerTransitionsModel payerTransitionsModel)
    {
        var item = await _payerTransitionRepository.GetById(payerTransitionsModel.PayerTransitionItem.Id);
        if (item is null)
        {
            payerTransitionsModel.IsNotValid = true;
            payerTransitionsModel.Message = "PayerTransition not found.";
            return payerTransitionsModel;
        }

        payerTransitionsModel.PayerTransitionItem = item;
        payerTransitionsModel.PayerTransitionResponse = _mapper.ToResponse(item);
        payerTransitionsModel.IsNotValid = false;
        payerTransitionsModel.Message = "PayerTransition retrieved successfully.";
        return payerTransitionsModel;
    }

    public async Task<PayerTransitionsModel> GetAll(PayerTransitionsModel payerTransitionsModel)
    {
        var items = await _payerTransitionRepository.GetAll();
        payerTransitionsModel.PayerTransitionItems = items;
        payerTransitionsModel.IsNotValid = false;
        payerTransitionsModel.Message = $"{items.Count} payer transition(s) retrieved.";
        return payerTransitionsModel;
    }

    public async Task<PayerTransitionsModel> Update(PayerTransitionsModel payerTransitionsModel)
    {
        var existing = await _payerTransitionRepository.GetById(payerTransitionsModel.PayerTransitionItem.Id);
        if (existing is null)
        {
            payerTransitionsModel.IsNotValid = true;
            payerTransitionsModel.Message = "PayerTransition not found.";
            return payerTransitionsModel;
        }

        var updatedItem = _mapper.ToItem(payerTransitionsModel.PayerTransitionRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _payerTransitionRepository.Update(updatedItem);
        payerTransitionsModel.PayerTransitionItem = savedItem!;
        payerTransitionsModel.PayerTransitionResponse = _mapper.ToResponse(savedItem!);
        payerTransitionsModel.IsNotValid = false;
        payerTransitionsModel.Message = "PayerTransition updated successfully.";
        return payerTransitionsModel;
    }

    public async Task<PayerTransitionsModel> Delete(PayerTransitionsModel payerTransitionsModel)
    {
        var deleted = await _payerTransitionRepository.Delete(payerTransitionsModel.PayerTransitionItem.Id);
        if (!deleted)
        {
            payerTransitionsModel.IsNotValid = true;
            payerTransitionsModel.Message = "PayerTransition not found.";
            return payerTransitionsModel;
        }

        payerTransitionsModel.IsNotValid = false;
        payerTransitionsModel.Message = "PayerTransition deleted successfully.";
        return payerTransitionsModel;
    }

    public async Task<ImportResult> Import(Stream csvStream) =>
        await RunImport(csvStream, FlushBatch);

    public async Task<ImportResult> ImportUpsert(Stream csvStream) =>
        await RunImport(csvStream, FlushUpsertBatch);

    private async Task<ImportResult> RunImport(Stream csvStream, Func<List<PayerTransitionItem>, ImportResult, Task> flush)
    {
        var result = new ImportResult();
        var batch = new List<PayerTransitionItem>();

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

    private async Task FlushBatch(List<PayerTransitionItem> batch, ImportResult result)
    {
        try
        {
            await _payerTransitionRepository.CreateBatch(batch);
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

    private async Task FlushUpsertBatch(List<PayerTransitionItem> batch, ImportResult result)
    {
        try
        {
            await _payerTransitionRepository.UpsertBatch(batch);
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
