using AI.HealthCare.Patient.Models.Encounter;
using EfEncounter = AI.HealthCare.Patient.EF.Entities.Encounter;

namespace AI.HealthCare.Patient.Repositories;

public interface IEncounterMapper
{
    EncounterItem ToModel(EfEncounter entity);
    EfEncounter ToEntity(EncounterItem item);
}
