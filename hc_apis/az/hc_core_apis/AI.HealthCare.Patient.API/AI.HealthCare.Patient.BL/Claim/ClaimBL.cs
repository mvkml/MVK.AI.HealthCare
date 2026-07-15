using AI.HealthCare.Patient.Models.Claim;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ClaimBL : IClaimBL
{
    private const int ImportBatchSize = 500;

    private readonly IClaimRepository _claimRepository;
    private readonly IClaimBLMapper _mapper;

    public ClaimBL(IClaimRepository claimRepository, IClaimBLMapper mapper)
    {
        _claimRepository = claimRepository;
        _mapper = mapper;
    }

    public async Task<ClaimsModel> Create(ClaimsModel claimsModel)
    {
        claimsModel.ClaimItem = _mapper.ToItem(claimsModel.ClaimRequest);
        claimsModel.ClaimItem.Id = Guid.NewGuid();

        var savedItem = await _claimRepository.Create(claimsModel.ClaimItem);
        claimsModel.ClaimItem = savedItem;

        claimsModel.ClaimResponse = _mapper.ToResponse(savedItem);
        claimsModel.IsNotValid = false;
        claimsModel.Message = "Claim created successfully.";

        return claimsModel;
    }

    public async Task<ClaimsModel> GetById(ClaimsModel claimsModel)
    {
        var item = await _claimRepository.GetById(claimsModel.ClaimItem.Id);
        if (item is null)
        {
            claimsModel.IsNotValid = true;
            claimsModel.Message = "Claim not found.";
            return claimsModel;
        }

        claimsModel.ClaimItem = item;
        claimsModel.ClaimResponse = _mapper.ToResponse(item);
        claimsModel.IsNotValid = false;
        claimsModel.Message = "Claim retrieved successfully.";
        return claimsModel;
    }

    public async Task<ClaimsModel> GetAll(ClaimsModel claimsModel)
    {
        var items = await _claimRepository.GetAll();
        claimsModel.ClaimItems = items;
        claimsModel.IsNotValid = false;
        claimsModel.Message = $"{items.Count} claim(s) retrieved.";
        return claimsModel;
    }

    public async Task<ClaimsModel> GetByPatientId(Guid patientId)
    {
        var items = await _claimRepository.GetByPatientId(patientId);
        return new ClaimsModel
        {
            ClaimItems = items,
            IsNotValid = false,
            Message = $"{items.Count} claim(s) retrieved for patient."
        };
    }

    public async Task<ClaimsModel> Update(ClaimsModel claimsModel)
    {
        var existing = await _claimRepository.GetById(claimsModel.ClaimItem.Id);
        if (existing is null)
        {
            claimsModel.IsNotValid = true;
            claimsModel.Message = "Claim not found.";
            return claimsModel;
        }

        var updatedItem = _mapper.ToItem(claimsModel.ClaimRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _claimRepository.Update(updatedItem);
        claimsModel.ClaimItem = savedItem!;
        claimsModel.ClaimResponse = _mapper.ToResponse(savedItem!);
        claimsModel.IsNotValid = false;
        claimsModel.Message = "Claim updated successfully.";
        return claimsModel;
    }

    public async Task<ClaimsModel> Delete(ClaimsModel claimsModel)
    {
        var deleted = await _claimRepository.Delete(claimsModel.ClaimItem.Id);
        if (!deleted)
        {
            claimsModel.IsNotValid = true;
            claimsModel.Message = "Claim not found.";
            return claimsModel;
        }

        claimsModel.IsNotValid = false;
        claimsModel.Message = "Claim deleted successfully.";
        return claimsModel;
    }

    public async Task<ImportResult> Import(Stream csvStream)
    {
        var result = new ImportResult();
        var batch = new List<ClaimItem>();

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

    private async Task FlushBatch(List<ClaimItem> batch, ImportResult result)
    {
        try
        {
            await _claimRepository.CreateBatch(batch);
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
