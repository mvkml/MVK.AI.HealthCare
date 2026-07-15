using AI.HealthCare.Patient.Models.Allergy;

namespace AI.HealthCare.Patient.BL;

public class AllergyValidationService : IAllergyValidationService
{
    public AllergiesModel Validate(AllergiesModel allergiesModel)
    {
        var request = allergiesModel.AllergyRequest;

        if (request.PatientId == Guid.Empty)
        {
            allergiesModel.IsNotValid = true;
            allergiesModel.Message = "PatientId is required.";
            return allergiesModel;
        }

        if (request.EncounterId == Guid.Empty)
        {
            allergiesModel.IsNotValid = true;
            allergiesModel.Message = "EncounterId is required.";
            return allergiesModel;
        }

        allergiesModel.IsNotValid = false;
        allergiesModel.Message = "Validation passed.";
        return allergiesModel;
    }
}
