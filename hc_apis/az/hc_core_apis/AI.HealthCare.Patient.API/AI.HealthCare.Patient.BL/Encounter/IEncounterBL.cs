using AI.HealthCare.Patient.Models.Encounter;
using AI.HealthCare.Patient.Models.Shared;

namespace AI.HealthCare.Patient.BL;

public interface IEncounterBL
{
    Task<EncountersModel> Create(EncountersModel encountersModel);
    Task<EncountersModel> GetById(EncountersModel encountersModel);
    Task<EncountersModel> GetAll(EncountersModel encountersModel);
    Task<EncountersModel> GetByPatientId(Guid patientId);
    Task<EncountersModel> Update(EncountersModel encountersModel);
    Task<EncountersModel> Delete(EncountersModel encountersModel);
    Task<ImportResult> Import(Stream csvStream);
    Task<ImportResult> ImportUpsert(Stream csvStream);
}
