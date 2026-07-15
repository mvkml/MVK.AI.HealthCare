using AI.HealthCare.Patient.Models.Allergy;

namespace AI.HealthCare.Patient.Repositories;

public interface IAllergyRepository
{
    Task<AllergyItem?> GetById(long id);
    Task<List<AllergyItem>> GetAll();
    Task<List<AllergyItem>> GetByPatientId(Guid patientId);
    Task<AllergyItem> Create(AllergyItem allergyItem);
    Task<AllergyItem?> Update(AllergyItem allergyItem);
    Task<bool> Delete(long id);
}
