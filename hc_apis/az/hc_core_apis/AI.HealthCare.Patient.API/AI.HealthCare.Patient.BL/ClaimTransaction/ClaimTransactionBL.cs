using AI.HealthCare.Patient.Models.ClaimTransaction;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ClaimTransactionBL : IClaimTransactionBL
{
    private const int ImportBatchSize = 500;

    private readonly IClaimTransactionRepository _claimTransactionRepository;
    private readonly IClaimTransactionBLMapper _mapper;

    public ClaimTransactionBL(IClaimTransactionRepository claimTransactionRepository, IClaimTransactionBLMapper mapper)
    {
        _claimTransactionRepository = claimTransactionRepository;
        _mapper = mapper;
    }

    public async Task<ClaimTransactionsModel> Create(ClaimTransactionsModel claimTransactionsModel)
    {
        claimTransactionsModel.ClaimTransactionItem = _mapper.ToItem(claimTransactionsModel.ClaimTransactionRequest);
        claimTransactionsModel.ClaimTransactionItem.Id = Guid.NewGuid();

        var savedItem = await _claimTransactionRepository.Create(claimTransactionsModel.ClaimTransactionItem);
        claimTransactionsModel.ClaimTransactionItem = savedItem;

        claimTransactionsModel.ClaimTransactionResponse = _mapper.ToResponse(savedItem);
        claimTransactionsModel.IsNotValid = false;
        claimTransactionsModel.Message = "ClaimTransaction created successfully.";

        return claimTransactionsModel;
    }

    public async Task<ClaimTransactionsModel> GetById(ClaimTransactionsModel claimTransactionsModel)
    {
        var item = await _claimTransactionRepository.GetById(claimTransactionsModel.ClaimTransactionItem.Id);
        if (item is null)
        {
            claimTransactionsModel.IsNotValid = true;
            claimTransactionsModel.Message = "ClaimTransaction not found.";
            return claimTransactionsModel;
        }

        claimTransactionsModel.ClaimTransactionItem = item;
        claimTransactionsModel.ClaimTransactionResponse = _mapper.ToResponse(item);
        claimTransactionsModel.IsNotValid = false;
        claimTransactionsModel.Message = "ClaimTransaction retrieved successfully.";
        return claimTransactionsModel;
    }

    public async Task<ClaimTransactionsModel> GetAll(ClaimTransactionsModel claimTransactionsModel)
    {
        var items = await _claimTransactionRepository.GetAll();
        claimTransactionsModel.ClaimTransactionItems = items;
        claimTransactionsModel.IsNotValid = false;
        claimTransactionsModel.Message = $"{items.Count} claim transaction(s) retrieved.";
        return claimTransactionsModel;
    }

    public async Task<ClaimTransactionsModel> GetByClaimId(Guid claimId)
    {
        var items = await _claimTransactionRepository.GetByClaimId(claimId);
        return new ClaimTransactionsModel
        {
            ClaimTransactionItems = items,
            IsNotValid = false,
            Message = $"{items.Count} claim transaction(s) retrieved for claim."
        };
    }

    public async Task<ClaimTransactionsModel> Update(ClaimTransactionsModel claimTransactionsModel)
    {
        var existing = await _claimTransactionRepository.GetById(claimTransactionsModel.ClaimTransactionItem.Id);
        if (existing is null)
        {
            claimTransactionsModel.IsNotValid = true;
            claimTransactionsModel.Message = "ClaimTransaction not found.";
            return claimTransactionsModel;
        }

        var updatedItem = _mapper.ToItem(claimTransactionsModel.ClaimTransactionRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _claimTransactionRepository.Update(updatedItem);
        claimTransactionsModel.ClaimTransactionItem = savedItem!;
        claimTransactionsModel.ClaimTransactionResponse = _mapper.ToResponse(savedItem!);
        claimTransactionsModel.IsNotValid = false;
        claimTransactionsModel.Message = "ClaimTransaction updated successfully.";
        return claimTransactionsModel;
    }

    public async Task<ClaimTransactionsModel> Delete(ClaimTransactionsModel claimTransactionsModel)
    {
        var deleted = await _claimTransactionRepository.Delete(claimTransactionsModel.ClaimTransactionItem.Id);
        if (!deleted)
        {
            claimTransactionsModel.IsNotValid = true;
            claimTransactionsModel.Message = "ClaimTransaction not found.";
            return claimTransactionsModel;
        }

        claimTransactionsModel.IsNotValid = false;
        claimTransactionsModel.Message = "ClaimTransaction deleted successfully.";
        return claimTransactionsModel;
    }

    public async Task<ImportResult> Import(Stream csvStream)
    {
        var result = new ImportResult();
        var batch = new List<ClaimTransactionItem>();

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

    private async Task FlushBatch(List<ClaimTransactionItem> batch, ImportResult result)
    {
        try
        {
            await _claimTransactionRepository.CreateBatch(batch);
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
