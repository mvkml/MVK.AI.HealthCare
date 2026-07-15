using AI.HealthCare.Patient.Models.Encounter;

namespace AI.HealthCare.Patient.Repositories;

public interface IEncounterRepository
{
    Task<EncounterItem?> GetById(Guid id);
    Task<List<EncounterItem>> GetAll();
    Task<List<EncounterItem>> GetByPatientId(Guid patientId);
    Task<EncounterItem> Create(EncounterItem encounterItem);
    Task CreateBatch(List<EncounterItem> encounterItems);
    Task<EncounterItem?> Update(EncounterItem encounterItem);
    Task<bool> Delete(Guid id);
}
