using AI.HealthCare.Patient.Models.Immunization;

namespace AI.HealthCare.Patient.Repositories;

public interface IImmunizationRepository
{
    Task<ImmunizationItem?> GetById(long id);
    Task<List<ImmunizationItem>> GetAll();
    Task<List<ImmunizationItem>> GetByPatientId(Guid patientId);
    Task<ImmunizationItem> Create(ImmunizationItem immunizationItem);
    Task<ImmunizationItem?> Update(ImmunizationItem immunizationItem);
    Task<bool> Delete(long id);
}
