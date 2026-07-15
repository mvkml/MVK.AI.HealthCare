using AI.HealthCare.Patient.Models.Encounter;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IEncounterBLMapper : ICsvRowParser<EncounterItem>
{
    EncounterItem ToItem(EncounterRequest request);
    EncounterResponse ToResponse(EncounterItem item);
}
