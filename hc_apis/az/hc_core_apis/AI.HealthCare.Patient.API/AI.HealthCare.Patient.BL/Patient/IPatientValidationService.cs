using AI.HealthCare.Patient.Models.Patient;

namespace AI.HealthCare.Patient.BL;

public interface IPatientValidationService
{
    PatientsModel Validate(PatientsModel patientsModel);
}
