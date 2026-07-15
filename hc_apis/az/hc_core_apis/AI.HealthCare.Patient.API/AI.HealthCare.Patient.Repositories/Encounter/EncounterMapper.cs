using AI.HealthCare.Patient.Models.Encounter;
using EfEncounter = AI.HealthCare.Patient.EF.Entities.Encounter;

namespace AI.HealthCare.Patient.Repositories;

public class EncounterMapper : IEncounterMapper
{
    public EncounterItem ToModel(EfEncounter entity) => new()
    {
        Id = entity.Id,
        Start = entity.Start,
        Stop = entity.Stop,
        PatientId = entity.PatientId,
        OrganizationId = entity.OrganizationId,
        ProviderId = entity.ProviderId,
        PayerId = entity.PayerId,
        EncounterClass = entity.EncounterClass,
        Code = entity.Code,
        Description = entity.Description,
        BaseEncounterCost = entity.BaseEncounterCost,
        TotalClaimCost = entity.TotalClaimCost,
        PayerCoverage = entity.PayerCoverage,
        ReasonCode = entity.ReasonCode,
        ReasonDescription = entity.ReasonDescription
    };

    public EfEncounter ToEntity(EncounterItem item) => new()
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
