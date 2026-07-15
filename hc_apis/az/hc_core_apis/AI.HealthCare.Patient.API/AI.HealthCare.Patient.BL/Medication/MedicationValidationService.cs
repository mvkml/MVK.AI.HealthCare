using AI.HealthCare.Patient.Models.Medication;

namespace AI.HealthCare.Patient.BL;

public class MedicationValidationService : IMedicationValidationService
{
    public MedicationsModel Validate(MedicationsModel medicationsModel)
    {
        var request = medicationsModel.MedicationRequest;

        if (request.PatientId == Guid.Empty)
        {
            medicationsModel.IsNotValid = true;
            medicationsModel.Message = "PatientId is required.";
            return medicationsModel;
        }

        if (request.EncounterId == Guid.Empty)
        {
            medicationsModel.IsNotValid = true;
            medicationsModel.Message = "EncounterId is required.";
            return medicationsModel;
        }

        medicationsModel.IsNotValid = false;
        medicationsModel.Message = "Validation passed.";
        return medicationsModel;
    }
}
