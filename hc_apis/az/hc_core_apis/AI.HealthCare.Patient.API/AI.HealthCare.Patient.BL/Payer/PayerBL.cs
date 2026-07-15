using AI.HealthCare.Patient.Models.Payer;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class PayerBL : IPayerBL
{
    private const int ImportBatchSize = 500;

    private readonly IPayerRepository _payerRepository;
    private readonly IPayerBLMapper _mapper;

    public PayerBL(IPayerRepository payerRepository, IPayerBLMapper mapper)
    {
        _payerRepository = payerRepository;
        _mapper = mapper;
    }

    public async Task<PayersModel> Create(PayersModel payersModel)
    {
        payersModel.PayerItem = _mapper.ToItem(payersModel.PayerRequest);
        payersModel.PayerItem.Id = Guid.NewGuid();

        var savedItem = await _payerRepository.Create(payersModel.PayerItem);
        payersModel.PayerItem = savedItem;

        payersModel.PayerResponse = _mapper.ToResponse(savedItem);
        payersModel.IsNotValid = false;
        payersModel.Message = "Payer created successfully.";

        return payersModel;
    }

    public async Task<PayersModel> GetById(PayersModel payersModel)
    {
        var item = await _payerRepository.GetById(payersModel.PayerItem.Id);
        if (item is null)
        {
            payersModel.IsNotValid = true;
            payersModel.Message = "Payer not found.";
            return payersModel;
        }

        payersModel.PayerItem = item;
        payersModel.PayerResponse = _mapper.ToResponse(item);
        payersModel.IsNotValid = false;
        payersModel.Message = "Payer retrieved successfully.";
        return payersModel;
    }

    public async Task<PayersModel> GetAll(PayersModel payersModel)
    {
        var items = await _payerRepository.GetAll();
        payersModel.PayerItems = items;
        payersModel.IsNotValid = false;
        payersModel.Message = $"{items.Count} payer(s) retrieved.";
        return payersModel;
    }

    public async Task<PayersModel> Update(PayersModel payersModel)
    {
        var existing = await _payerRepository.GetById(payersModel.PayerItem.Id);
        if (existing is null)
        {
            payersModel.IsNotValid = true;
            payersModel.Message = "Payer not found.";
            return payersModel;
        }

        var updatedItem = _mapper.ToItem(payersModel.PayerRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _payerRepository.Update(updatedItem);
        payersModel.PayerItem = savedItem!;
        payersModel.PayerResponse = _mapper.ToResponse(savedItem!);
        payersModel.IsNotValid = false;
        payersModel.Message = "Payer updated successfully.";
        return payersModel;
    }

    public async Task<PayersModel> Delete(PayersModel payersModel)
    {
        var deleted = await _payerRepository.Delete(payersModel.PayerItem.Id);
        if (!deleted)
        {
            payersModel.IsNotValid = true;
            payersModel.Message = "Payer not found.";
            return payersModel;
        }

        payersModel.IsNotValid = false;
        payersModel.Message = "Payer deleted successfully.";
        return payersModel;
    }

    public async Task<ImportResult> Import(Stream csvStream) =>
        await RunImport(csvStream, FlushBatch);

    public async Task<ImportResult> ImportUpsert(Stream csvStream) =>
        await RunImport(csvStream, FlushUpsertBatch);

    private async Task<ImportResult> RunImport(Stream csvStream, Func<List<PayerItem>, ImportResult, Task> flush)
    {
        var result = new ImportResult();
        var batch = new List<PayerItem>();

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

    private async Task FlushBatch(List<PayerItem> batch, ImportResult result)
    {
        try
        {
            await _payerRepository.CreateBatch(batch);
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

    private async Task FlushUpsertBatch(List<PayerItem> batch, ImportResult result)
    {
        try
        {
            await _payerRepository.UpsertBatch(batch);
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
