using AI.HealthCare.Patient.Models.Medication;

namespace AI.HealthCare.Patient.Repositories;

public interface IMedicationRepository
{
    Task<MedicationItem?> GetById(long id);
    Task<List<MedicationItem>> GetAll();
    Task<List<MedicationItem>> GetByPatientId(Guid patientId);
    Task<MedicationItem> Create(MedicationItem medicationItem);
    Task CreateBatch(List<MedicationItem> medicationItems);
    Task UpsertBatch(List<MedicationItem> medicationItems);
    Task<MedicationItem?> Update(MedicationItem medicationItem);
    Task<bool> Delete(long id);
}
