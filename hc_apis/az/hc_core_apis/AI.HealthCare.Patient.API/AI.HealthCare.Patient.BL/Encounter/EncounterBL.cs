using AI.HealthCare.Patient.Models.Encounter;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class EncounterBL : IEncounterBL
{
    private readonly IEncounterRepository _encounterRepository;

    public EncounterBL(IEncounterRepository encounterRepository)
    {
        _encounterRepository = encounterRepository;
    }

    public async Task<EncountersModel> Create(EncountersModel encountersModel)
    {
        encountersModel.EncounterItem = ToItem(encountersModel.EncounterRequest);
        encountersModel.EncounterItem.Id = Guid.NewGuid();

        var savedItem = await _encounterRepository.Create(encountersModel.EncounterItem);
        encountersModel.EncounterItem = savedItem;

        encountersModel.EncounterResponse = ToResponse(savedItem);
        encountersModel.IsNotValid = false;
        encountersModel.Message = "Encounter created successfully.";

        return encountersModel;
    }

    public async Task<EncountersModel> GetById(EncountersModel encountersModel)
    {
        var item = await _encounterRepository.GetById(encountersModel.EncounterItem.Id);
        if (item is null)
        {
            encountersModel.IsNotValid = true;
            encountersModel.Message = "Encounter not found.";
            return encountersModel;
        }

        encountersModel.EncounterItem = item;
        encountersModel.EncounterResponse = ToResponse(item);
        encountersModel.IsNotValid = false;
        encountersModel.Message = "Encounter retrieved successfully.";
        return encountersModel;
    }

    public async Task<EncountersModel> GetAll(EncountersModel encountersModel)
    {
        var items = await _encounterRepository.GetAll();
        encountersModel.EncounterItems = items;
        encountersModel.IsNotValid = false;
        encountersModel.Message = $"{items.Count} encounter(s) retrieved.";
        return encountersModel;
    }

    public async Task<EncountersModel> GetByPatientId(Guid patientId)
    {
        var items = await _encounterRepository.GetByPatientId(patientId);
        return new EncountersModel
        {
            EncounterItems = items,
            IsNotValid = false,
            Message = $"{items.Count} encounter(s) retrieved for patient."
        };
    }

    public async Task<EncountersModel> Update(EncountersModel encountersModel)
    {
        var existing = await _encounterRepository.GetById(encountersModel.EncounterItem.Id);
        if (existing is null)
        {
            encountersModel.IsNotValid = true;
            encountersModel.Message = "Encounter not found.";
            return encountersModel;
        }

        var updatedItem = ToItem(encountersModel.EncounterRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _encounterRepository.Update(updatedItem);
        encountersModel.EncounterItem = savedItem!;
        encountersModel.EncounterResponse = ToResponse(savedItem!);
        encountersModel.IsNotValid = false;
        encountersModel.Message = "Encounter updated successfully.";
        return encountersModel;
    }

    public async Task<EncountersModel> Delete(EncountersModel encountersModel)
    {
        var deleted = await _encounterRepository.Delete(encountersModel.EncounterItem.Id);
        if (!deleted)
        {
            encountersModel.IsNotValid = true;
            encountersModel.Message = "Encounter not found.";
            return encountersModel;
        }

        encountersModel.IsNotValid = false;
        encountersModel.Message = "Encounter deleted successfully.";
        return encountersModel;
    }

    private static EncounterItem ToItem(EncounterRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        OrganizationId = request.OrganizationId,
        ProviderId = request.ProviderId,
        PayerId = request.PayerId,
        EncounterClass = request.EncounterClass,
        Code = request.Code,
        Description = request.Description,
        BaseEncounterCost = request.BaseEncounterCost,
        TotalClaimCost = request.TotalClaimCost,
        PayerCoverage = request.PayerCoverage,
        ReasonCode = request.ReasonCode,
        ReasonDescription = request.ReasonDescription
    };

    private static EncounterResponse ToResponse(EncounterItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        OrganizationId = item.OrganizationId,
        ProviderId = item.ProviderId,
        PayerId = item.PayerId,
        EncounterClass = item.EncounterClass,
        Code = item.Code,
        Description = item.Description,
        BaseEncounterCost = item.BaseEncounterCost,
        TotalClaimCost = item.TotalClaimCost,
        PayerCoverage = item.PayerCoverage,
        ReasonCode = item.ReasonCode,
        ReasonDescription = item.ReasonDescription
    };
}
