using AI.HealthCare.Patient.Models.Provider;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ProviderBL : IProviderBL
{
    private const int ImportBatchSize = 500;

    private readonly IProviderRepository _providerRepository;
    private readonly IProviderBLMapper _mapper;

    public ProviderBL(IProviderRepository providerRepository, IProviderBLMapper mapper)
    {
        _providerRepository = providerRepository;
        _mapper = mapper;
    }

    public async Task<ProvidersModel> Create(ProvidersModel providersModel)
    {
        providersModel.ProviderItem = _mapper.ToItem(providersModel.ProviderRequest);
        providersModel.ProviderItem.Id = Guid.NewGuid();

        var savedItem = await _providerRepository.Create(providersModel.ProviderItem);
        providersModel.ProviderItem = savedItem;

        providersModel.ProviderResponse = _mapper.ToResponse(savedItem);
        providersModel.IsNotValid = false;
        providersModel.Message = "Provider created successfully.";

        return providersModel;
    }

    public async Task<ProvidersModel> GetById(ProvidersModel providersModel)
    {
        var item = await _providerRepository.GetById(providersModel.ProviderItem.Id);
        if (item is null)
        {
            providersModel.IsNotValid = true;
            providersModel.Message = "Provider not found.";
            return providersModel;
        }

        providersModel.ProviderItem = item;
        providersModel.ProviderResponse = _mapper.ToResponse(item);
        providersModel.IsNotValid = false;
        providersModel.Message = "Provider retrieved successfully.";
        return providersModel;
    }

    public async Task<ProvidersModel> GetAll(ProvidersModel providersModel)
    {
        var items = await _providerRepository.GetAll();
        providersModel.ProviderItems = items;
        providersModel.IsNotValid = false;
        providersModel.Message = $"{items.Count} provider(s) retrieved.";
        return providersModel;
    }

    public async Task<ProvidersModel> Update(ProvidersModel providersModel)
    {
        var existing = await _providerRepository.GetById(providersModel.ProviderItem.Id);
        if (existing is null)
        {
            providersModel.IsNotValid = true;
            providersModel.Message = "Provider not found.";
            return providersModel;
        }

        var updatedItem = _mapper.ToItem(providersModel.ProviderRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _providerRepository.Update(updatedItem);
        providersModel.ProviderItem = savedItem!;
        providersModel.ProviderResponse = _mapper.ToResponse(savedItem!);
        providersModel.IsNotValid = false;
        providersModel.Message = "Provider updated successfully.";
        return providersModel;
    }

    public async Task<ProvidersModel> Delete(ProvidersModel providersModel)
    {
        var deleted = await _providerRepository.Delete(providersModel.ProviderItem.Id);
        if (!deleted)
        {
            providersModel.IsNotValid = true;
            providersModel.Message = "Provider not found.";
            return providersModel;
        }

        providersModel.IsNotValid = false;
        providersModel.Message = "Provider deleted successfully.";
        return providersModel;
    }

    public async Task<ImportResult> Import(Stream csvStream) =>
        await RunImport(csvStream, FlushBatch);

    public async Task<ImportResult> ImportUpsert(Stream csvStream) =>
        await RunImport(csvStream, FlushUpsertBatch);

    private async Task<ImportResult> RunImport(Stream csvStream, Func<List<ProviderItem>, ImportResult, Task> flush)
    {
        var result = new ImportResult();
        var batch = new List<ProviderItem>();

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

    private async Task FlushBatch(List<ProviderItem> batch, ImportResult result)
    {
        try
        {
            await _providerRepository.CreateBatch(batch);
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

    private async Task FlushUpsertBatch(List<ProviderItem> batch, ImportResult result)
    {
        try
        {
            await _providerRepository.UpsertBatch(batch);
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
