using AI.HealthCare.Patient.Models.Immunization;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ImmunizationBL : IImmunizationBL
{
    private readonly IImmunizationRepository _immunizationRepository;

    public ImmunizationBL(IImmunizationRepository immunizationRepository)
    {
        _immunizationRepository = immunizationRepository;
    }

    public async Task<ImmunizationsModel> Create(ImmunizationsModel immunizationsModel)
    {
        immunizationsModel.ImmunizationItem = ToItem(immunizationsModel.ImmunizationRequest);

        var savedItem = await _immunizationRepository.Create(immunizationsModel.ImmunizationItem);
        immunizationsModel.ImmunizationItem = savedItem;

        immunizationsModel.ImmunizationResponse = ToResponse(savedItem);
        immunizationsModel.IsNotValid = false;
        immunizationsModel.Message = "Immunization created successfully.";

        return immunizationsModel;
    }

    public async Task<ImmunizationsModel> GetById(ImmunizationsModel immunizationsModel)
    {
        var item = await _immunizationRepository.GetById(immunizationsModel.ImmunizationItem.Id);
        if (item is null)
        {
            immunizationsModel.IsNotValid = true;
            immunizationsModel.Message = "Immunization not found.";
            return immunizationsModel;
        }

        immunizationsModel.ImmunizationItem = item;
        immunizationsModel.ImmunizationResponse = ToResponse(item);
        immunizationsModel.IsNotValid = false;
        immunizationsModel.Message = "Immunization retrieved successfully.";
        return immunizationsModel;
    }

    public async Task<ImmunizationsModel> GetAll(ImmunizationsModel immunizationsModel)
    {
        var items = await _immunizationRepository.GetAll();
        immunizationsModel.ImmunizationItems = items;
        immunizationsModel.IsNotValid = false;
        immunizationsModel.Message = $"{items.Count} immunization(s) retrieved.";
        return immunizationsModel;
    }

    public async Task<ImmunizationsModel> GetByPatientId(Guid patientId)
    {
        var items = await _immunizationRepository.GetByPatientId(patientId);
        return new ImmunizationsModel
        {
            ImmunizationItems = items,
            IsNotValid = false,
            Message = $"{items.Count} immunization(s) retrieved for patient."
        };
    }

    public async Task<ImmunizationsModel> Update(ImmunizationsModel immunizationsModel)
    {
        var existing = await _immunizationRepository.GetById(immunizationsModel.ImmunizationItem.Id);
        if (existing is null)
        {
            immunizationsModel.IsNotValid = true;
            immunizationsModel.Message = "Immunization not found.";
            return immunizationsModel;
        }

        var updatedItem = ToItem(immunizationsModel.ImmunizationRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _immunizationRepository.Update(updatedItem);
        immunizationsModel.ImmunizationItem = savedItem!;
        immunizationsModel.ImmunizationResponse = ToResponse(savedItem!);
        immunizationsModel.IsNotValid = false;
        immunizationsModel.Message = "Immunization updated successfully.";
        return immunizationsModel;
    }

    public async Task<ImmunizationsModel> Delete(ImmunizationsModel immunizationsModel)
    {
        var deleted = await _immunizationRepository.Delete(immunizationsModel.ImmunizationItem.Id);
        if (!deleted)
        {
            immunizationsModel.IsNotValid = true;
            immunizationsModel.Message = "Immunization not found.";
            return immunizationsModel;
        }

        immunizationsModel.IsNotValid = false;
        immunizationsModel.Message = "Immunization deleted successfully.";
        return immunizationsModel;
    }

    private static ImmunizationItem ToItem(ImmunizationRequest request) => new()
    {
        Date = request.Date,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        Code = request.Code,
        Description = request.Description,
        BaseCost = request.BaseCost
    };

    private static ImmunizationResponse ToResponse(ImmunizationItem item) => new()
    {
        Id = item.Id,
        Date = item.Date,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        Description = item.Description,
        BaseCost = item.BaseCost
    };
}
