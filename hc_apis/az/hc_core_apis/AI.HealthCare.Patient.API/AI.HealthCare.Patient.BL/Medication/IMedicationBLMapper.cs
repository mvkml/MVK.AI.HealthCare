using AI.HealthCare.Patient.Models.Medication;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IMedicationBLMapper : ICsvRowParser<MedicationItem>
{
    MedicationItem ToItem(MedicationRequest request);
    MedicationResponse ToResponse(MedicationItem item);
}
