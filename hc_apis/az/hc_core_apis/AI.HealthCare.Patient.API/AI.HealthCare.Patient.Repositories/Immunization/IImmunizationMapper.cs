using AI.HealthCare.Patient.Models.Immunization;
using EfImmunization = AI.HealthCare.Patient.EF.Entities.Immunization;

namespace AI.HealthCare.Patient.Repositories;

public interface IImmunizationMapper
{
    ImmunizationItem ToModel(EfImmunization entity);
    EfImmunization ToEntity(ImmunizationItem item);
}
