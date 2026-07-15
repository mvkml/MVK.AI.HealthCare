using AI.HealthCare.Patient.Models.Allergy;

namespace AI.HealthCare.Patient.BL;

public interface IAllergyValidationService
{
    AllergiesModel Validate(AllergiesModel allergiesModel);
}
