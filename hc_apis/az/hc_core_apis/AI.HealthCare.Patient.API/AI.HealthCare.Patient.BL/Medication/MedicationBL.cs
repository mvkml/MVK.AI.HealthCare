using AI.HealthCare.Patient.Models.Medication;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class MedicationBL : IMedicationBL
{
    private readonly IMedicationRepository _medicationRepository;

    public MedicationBL(IMedicationRepository medicationRepository)
    {
        _medicationRepository = medicationRepository;
    }

    public async Task<MedicationsModel> Create(MedicationsModel medicationsModel)
    {
        medicationsModel.MedicationItem = ToItem(medicationsModel.MedicationRequest);

        var savedItem = await _medicationRepository.Create(medicationsModel.MedicationItem);
        medicationsModel.MedicationItem = savedItem;

        medicationsModel.MedicationResponse = ToResponse(savedItem);
        medicationsModel.IsNotValid = false;
        medicationsModel.Message = "Medication created successfully.";

        return medicationsModel;
    }

    public async Task<MedicationsModel> GetById(MedicationsModel medicationsModel)
    {
        var item = await _medicationRepository.GetById(medicationsModel.MedicationItem.Id);
        if (item is null)
        {
            medicationsModel.IsNotValid = true;
            medicationsModel.Message = "Medication not found.";
            return medicationsModel;
        }

        medicationsModel.MedicationItem = item;
        medicationsModel.MedicationResponse = ToResponse(item);
        medicationsModel.IsNotValid = false;
        medicationsModel.Message = "Medication retrieved successfully.";
        return medicationsModel;
    }

    public async Task<MedicationsModel> GetAll(MedicationsModel medicationsModel)
    {
        var items = await _medicationRepository.GetAll();
        medicationsModel.MedicationItems = items;
        medicationsModel.IsNotValid = false;
        medicationsModel.Message = $"{items.Count} medication(s) retrieved.";
        return medicationsModel;
    }

    public async Task<MedicationsModel> GetByPatientId(Guid patientId)
    {
        var items = await _medicationRepository.GetByPatientId(patientId);
        return new MedicationsModel
        {
            MedicationItems = items,
            IsNotValid = false,
            Message = $"{items.Count} medication(s) retrieved for patient."
        };
    }

    public async Task<MedicationsModel> Update(MedicationsModel medicationsModel)
    {
        var existing = await _medicationRepository.GetById(medicationsModel.MedicationItem.Id);
        if (existing is null)
        {
            medicationsModel.IsNotValid = true;
            medicationsModel.Message = "Medication not found.";
            return medicationsModel;
        }

        var updatedItem = ToItem(medicationsModel.MedicationRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _medicationRepository.Update(updatedItem);
        medicationsModel.MedicationItem = savedItem!;
        medicationsModel.MedicationResponse = ToResponse(savedItem!);
        medicationsModel.IsNotValid = false;
        medicationsModel.Message = "Medication updated successfully.";
        return medicationsModel;
    }

    public async Task<MedicationsModel> Delete(MedicationsModel medicationsModel)
    {
        var deleted = await _medicationRepository.Delete(medicationsModel.MedicationItem.Id);
        if (!deleted)
        {
            medicationsModel.IsNotValid = true;
            medicationsModel.Message = "Medication not found.";
            return medicationsModel;
        }

        medicationsModel.IsNotValid = false;
        medicationsModel.Message = "Medication deleted successfully.";
        return medicationsModel;
    }

    private static MedicationItem ToItem(MedicationRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        PayerId = request.PayerId,
        EncounterId = request.EncounterId,
        Code = request.Code,
        Description = request.Description,
        BaseCost = request.BaseCost,
        PayerCoverage = request.PayerCoverage,
        TotalCost = request.TotalCost,
        Dispenses = request.Dispenses,
        ReasonCode = request.ReasonCode,
        ReasonDescription = request.ReasonDescription
    };

    private static MedicationResponse ToResponse(MedicationItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        PayerId = item.PayerId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        Description = item.Description,
        BaseCost = item.BaseCost,
        PayerCoverage = item.PayerCoverage,
        TotalCost = item.TotalCost,
        Dispenses = item.Dispenses,
        ReasonCode = item.ReasonCode,
        ReasonDescription = item.ReasonDescription
    };
}
