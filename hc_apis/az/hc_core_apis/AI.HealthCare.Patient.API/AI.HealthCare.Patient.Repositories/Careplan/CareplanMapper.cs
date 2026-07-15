using AI.HealthCare.Patient.Models.Careplan;
using EfCareplan = AI.HealthCare.Patient.EF.Entities.Careplan;

namespace AI.HealthCare.Patient.Repositories;

public class CareplanMapper : ICareplanMapper
{
    public CareplanItem ToModel(EfCareplan entity) => new()
    {
        Id = entity.Id,
        Start = entity.Start,
        Stop = entity.Stop,
        PatientId = entity.PatientId,
        EncounterId = entity.EncounterId,
        Code = entity.Code,
        Description = entity.Description,
        ReasonCode = entity.ReasonCode,
        ReasonDescription = entity.ReasonDescription
    };

    public EfCareplan ToEntity(CareplanItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        Description = item.Description,
        ReasonCode = item.ReasonCode,
        ReasonDescription = item.ReasonDescription
    };
}
