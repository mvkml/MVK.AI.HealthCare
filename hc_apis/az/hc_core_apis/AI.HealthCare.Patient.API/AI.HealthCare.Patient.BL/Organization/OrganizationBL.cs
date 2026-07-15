using AI.HealthCare.Patient.Models.Organization;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class OrganizationBL : IOrganizationBL
{
    private readonly IOrganizationRepository _organizationRepository;

    public OrganizationBL(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<OrganizationsModel> Create(OrganizationsModel organizationsModel)
    {
        organizationsModel.OrganizationItem = ToItem(organizationsModel.OrganizationRequest);
        organizationsModel.OrganizationItem.Id = Guid.NewGuid();

        var savedItem = await _organizationRepository.Create(organizationsModel.OrganizationItem);
        organizationsModel.OrganizationItem = savedItem;

        organizationsModel.OrganizationResponse = ToResponse(savedItem);
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
        organizationsModel.OrganizationResponse = ToResponse(item);
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

        var updatedItem = ToItem(organizationsModel.OrganizationRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _organizationRepository.Update(updatedItem);
        organizationsModel.OrganizationItem = savedItem!;
        organizationsModel.OrganizationResponse = ToResponse(savedItem!);
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

    private static OrganizationItem ToItem(OrganizationRequest request) => new()
    {
        Name = request.Name,
        Address = request.Address,
        City = request.City,
        State = request.State,
        Zip = request.Zip,
        Phone = request.Phone,
        Lat = request.Lat,
        Lon = request.Lon,
        Revenue = request.Revenue,
        Utilization = request.Utilization
    };

    private static OrganizationResponse ToResponse(OrganizationItem item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Address = item.Address,
        City = item.City,
        State = item.State,
        Zip = item.Zip,
        Phone = item.Phone,
        Lat = item.Lat,
        Lon = item.Lon,
        Revenue = item.Revenue,
        Utilization = item.Utilization
    };
}
