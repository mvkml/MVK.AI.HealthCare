using AI.HealthCare.Patient.Models.Medication;
using EfMedication = AI.HealthCare.Patient.EF.Entities.Medication;

namespace AI.HealthCare.Patient.Repositories;

public class MedicationMapper : IMedicationMapper
{
    public MedicationItem ToModel(EfMedication entity) => new()
    {
        Id = entity.Id,
        Start = entity.Start,
        Stop = entity.Stop,
        PatientId = entity.PatientId,
        PayerId = entity.PayerId,
        EncounterId = entity.EncounterId,
        Code = entity.Code,
        Description = entity.Description,
        BaseCost = entity.BaseCost,
        PayerCoverage = entity.PayerCoverage,
        TotalCost = entity.TotalCost,
        Dispenses = entity.Dispenses,
        ReasonCode = entity.ReasonCode,
        ReasonDescription = entity.ReasonDescription
    };

    public EfMedication ToEntity(MedicationItem item) => new()
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
