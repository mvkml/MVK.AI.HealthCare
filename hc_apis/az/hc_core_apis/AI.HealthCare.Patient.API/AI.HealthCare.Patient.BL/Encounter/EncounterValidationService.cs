using AI.HealthCare.Patient.Models.Encounter;

namespace AI.HealthCare.Patient.BL;

public class EncounterValidationService : IEncounterValidationService
{
    public EncountersModel Validate(EncountersModel encountersModel)
    {
        var request = encountersModel.EncounterRequest;

        if (request.PatientId == Guid.Empty)
        {
            encountersModel.IsNotValid = true;
            encountersModel.Message = "PatientId is required.";
            return encountersModel;
        }

        if (request.OrganizationId == Guid.Empty)
        {
            encountersModel.IsNotValid = true;
            encountersModel.Message = "OrganizationId is required.";
            return encountersModel;
        }

        if (request.ProviderId == Guid.Empty)
        {
            encountersModel.IsNotValid = true;
            encountersModel.Message = "ProviderId is required.";
            return encountersModel;
        }

        encountersModel.IsNotValid = false;
        encountersModel.Message = "Validation passed.";
        return encountersModel;
    }
}
