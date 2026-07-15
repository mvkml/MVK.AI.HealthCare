using AI.HealthCare.Patient.Models.Encounter;

namespace AI.HealthCare.Patient.BL;

public interface IEncounterValidationService
{
    EncountersModel Validate(EncountersModel encountersModel);
}
