using AI.HealthCare.Patient.Models.Patient;

namespace AI.HealthCare.Patient.BL;

public class PatientValidationService : IPatientValidationService
{
    public PatientsModel Validate(PatientsModel patientsModel)
    {
        var request = patientsModel.PatientRequest;

        if (string.IsNullOrWhiteSpace(request.First))
        {
            patientsModel.IsNotValid = true;
            patientsModel.Message = "First name is required.";
            return patientsModel;
        }

        if (string.IsNullOrWhiteSpace(request.Last))
        {
            patientsModel.IsNotValid = true;
            patientsModel.Message = "Last name is required.";
            return patientsModel;
        }

        if (request.BirthDate == default || request.BirthDate > DateTime.UtcNow)
        {
            patientsModel.IsNotValid = true;
            patientsModel.Message = "A valid BirthDate is required.";
            return patientsModel;
        }

        if (request.DeathDate.HasValue && request.DeathDate < request.BirthDate)
        {
            patientsModel.IsNotValid = true;
            patientsModel.Message = "DeathDate cannot be earlier than BirthDate.";
            return patientsModel;
        }

        patientsModel.IsNotValid = false;
        patientsModel.Message = "Validation passed.";
        return patientsModel;
    }
}
