using AI.HealthCare.Patient.Models.Medication;

namespace AI.HealthCare.Patient.BL;

public interface IMedicationValidationService
{
    MedicationsModel Validate(MedicationsModel medicationsModel);
}
