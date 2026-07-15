using AI.HealthCare.Patient.Models.Organization;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class OrganizationBL : IOrganizationBL
{
    private const int ImportBatchSize = 500;

    private readonly IOrganizationRepository _organizationRepository;
    private readonly IOrganizationBLMapper _mapper;

    public OrganizationBL(IOrganizationRepository organizationRepository, IOrganizationBLMapper mapper)
    {
        _organizationRepository = organizationRepository;
        _mapper = mapper;
    }

    public async Task<OrganizationsModel> Create(OrganizationsModel organizationsModel)
    {
        organizationsModel.OrganizationItem = _mapper.ToItem(organizationsModel.OrganizationRequest);
        organizationsModel.OrganizationItem.Id = Guid.NewGuid();

        var savedItem = await _organizationRepository.Create(organizationsModel.OrganizationItem);
        organizationsModel.OrganizationItem = savedItem;

        organizationsModel.OrganizationResponse = _mapper.ToResponse(savedItem);
        organizationsModel.IsNotValid = false;
        organizationsModel.Message = "Organization created successfully.";

        return organizationsModel;
    }

    public async Task<OrganizationsModel> GetById(OrganizationsModel organizationsModel)
    {
        var item = await _organizationRepository.GetById(organizationsModel.OrganizationItem.Id);
        if (item is null)
        {
            organizationsModel.IsNotValid = true;
            organizationsModel.Message = "Organization not found.";
            return organizationsModel;
        }

        organizationsModel.OrganizationItem = item;
        organizationsModel.OrganizationResponse = _mapper.ToResponse(item);
        organizationsModel.IsNotValid = false;
        organizationsModel.Message = "Organization retrieved successfully.";
        return organizationsModel;
    }

    public async Task<OrganizationsModel> GetAll(OrganizationsModel organizationsModel)
    {
        var items = await _organizationRepository.GetAll();
        organizationsModel.OrganizationItems = items;
        organizationsModel.IsNotValid = false;
        organizationsModel.Message = $"{items.Count} organization(s) retrieved.";
        return organizationsModel;
    }

    public async Task<OrganizationsModel> Update(OrganizationsModel organizationsModel)
    {
        var existing = await _organizationRepository.GetById(organizationsModel.OrganizationItem.Id);
        if (existing is null)
        {
            organizationsModel.IsNotValid = true;
            organizationsModel.Message = "Organization not found.";
            return organizationsModel;
        }

        var updatedItem = _mapper.ToItem(organizationsModel.OrganizationRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _organizationRepository.Update(updatedItem);
        organizationsModel.OrganizationItem = savedItem!;
        organizationsModel.OrganizationResponse = _mapper.ToResponse(savedItem!);
        organizationsModel.IsNotValid = false;
        organizationsModel.Message = "Organization updated successfully.";
        return organizationsModel;
    }

    public async Task<OrganizationsModel> Delete(OrganizationsModel organizationsModel)
    {
        var deleted = await _organizationRepository.Delete(organizationsModel.OrganizationItem.Id);
        if (!deleted)
        {
            organizationsModel.IsNotValid = true;
            organizationsModel.Message = "Organization not found.";
            return organizationsModel;
        }

        organizationsModel.IsNotValid = false;
        organizationsModel.Message = "Organization deleted successfully.";
        return organizationsModel;
    }

    public async Task<ImportResult> Import(Stream csvStream)
    {
        var result = new ImportResult();
        var batch = new List<OrganizationItem>();

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

    private async Task FlushBatch(List<OrganizationItem> batch, ImportResult result)
    {
        try
        {
            await _organizationRepository.CreateBatch(batch);
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
