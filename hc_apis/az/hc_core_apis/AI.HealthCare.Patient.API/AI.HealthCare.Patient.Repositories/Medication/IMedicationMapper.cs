using AI.HealthCare.Patient.Models.Medication;
using EfMedication = AI.HealthCare.Patient.EF.Entities.Medication;

namespace AI.HealthCare.Patient.Repositories;

public interface IMedicationMapper
{
    MedicationItem ToModel(EfMedication entity);
    EfMedication ToEntity(MedicationItem item);
}
