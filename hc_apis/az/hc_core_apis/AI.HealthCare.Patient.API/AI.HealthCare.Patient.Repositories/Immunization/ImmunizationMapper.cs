using AI.HealthCare.Patient.Models.Immunization;
using EfImmunization = AI.HealthCare.Patient.EF.Entities.Immunization;

namespace AI.HealthCare.Patient.Repositories;

public class ImmunizationMapper : IImmunizationMapper
{
    public ImmunizationItem ToModel(EfImmunization entity) => new()
    {
        Id = entity.Id,
        Date = entity.Date,
        PatientId = entity.PatientId,
        EncounterId = entity.EncounterId,
        Code = entity.Code,
        Description = entity.Description,
        BaseCost = entity.BaseCost
    };

    public EfImmunization ToEntity(ImmunizationItem item) => new()
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
