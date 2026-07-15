using AI.HealthCare.Patient.Models.Allergy;
using EfAllergy = AI.HealthCare.Patient.EF.Entities.Allergy;

namespace AI.HealthCare.Patient.Repositories;

public interface IAllergyMapper
{
    AllergyItem ToModel(EfAllergy entity);
    EfAllergy ToEntity(AllergyItem item);
}
