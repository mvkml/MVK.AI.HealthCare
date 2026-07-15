using AI.HealthCare.Patient.Models.Patient;

namespace AI.HealthCare.Patient.Repositories;

public interface IPatientRepository
{
    Task<PatientItem?> GetById(Guid id);
    Task<List<PatientItem>> GetAll();
    Task<PatientItem> Create(PatientItem patientItem);
    Task<PatientItem?> Update(PatientItem patientItem);
    Task<bool> Delete(Guid id);
}
