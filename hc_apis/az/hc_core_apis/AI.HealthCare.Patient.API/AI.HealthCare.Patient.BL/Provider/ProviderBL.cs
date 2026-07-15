using AI.HealthCare.Patient.Models.Provider;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ProviderBL : IProviderBL
{
    private readonly IProviderRepository _providerRepository;

    public ProviderBL(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<ProvidersModel> Create(ProvidersModel providersModel)
    {
        providersModel.ProviderItem = ToItem(providersModel.ProviderRequest);
        providersModel.ProviderItem.Id = Guid.NewGuid();

        var savedItem = await _providerRepository.Create(providersModel.ProviderItem);
        providersModel.ProviderItem = savedItem;

        providersModel.ProviderResponse = ToResponse(savedItem);
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
        providersModel.ProviderResponse = ToResponse(item);
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

        var updatedItem = ToItem(providersModel.ProviderRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _providerRepository.Update(updatedItem);
        providersModel.ProviderItem = savedItem!;
        providersModel.ProviderResponse = ToResponse(savedItem!);
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

    private static ProviderItem ToItem(ProviderRequest request) => new()
    {
        OrganizationId = request.OrganizationId,
        Name = request.Name,
        Gender = request.Gender,
        Speciality = request.Speciality,
        Address = request.Address,
        City = request.City,
        State = request.State,
        Zip = request.Zip,
        Lat = request.Lat,
        Lon = request.Lon,
        Encounters = request.Encounters,
        Procedures = request.Procedures
    };

    private static ProviderResponse ToResponse(ProviderItem item) => new()
    {
        Id = item.Id,
        OrganizationId = item.OrganizationId,
        Name = item.Name,
        Gender = item.Gender,
        Speciality = item.Speciality,
        Address = item.Address,
        City = item.City,
        State = item.State,
        Zip = item.Zip,
        Lat = item.Lat,
        Lon = item.Lon,
        Encounters = item.Encounters,
        Procedures = item.Procedures
    };
}
