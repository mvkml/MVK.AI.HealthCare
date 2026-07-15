using AI.HealthCare.Patient.Models.Immunization;

namespace AI.HealthCare.Patient.BL;

public class ImmunizationValidationService : IImmunizationValidationService
{
    public ImmunizationsModel Validate(ImmunizationsModel immunizationsModel)
    {
        var request = immunizationsModel.ImmunizationRequest;

        if (request.PatientId == Guid.Empty)
        {
            immunizationsModel.IsNotValid = true;
            immunizationsModel.Message = "PatientId is required.";
            return immunizationsModel;
        }

        if (request.EncounterId == Guid.Empty)
        {
            immunizationsModel.IsNotValid = true;
            immunizationsModel.Message = "EncounterId is required.";
            return immunizationsModel;
        }

        if (request.Date == default)
        {
            immunizationsModel.IsNotValid = true;
            immunizationsModel.Message = "Date is required.";
            return immunizationsModel;
        }

        immunizationsModel.IsNotValid = false;
        immunizationsModel.Message = "Validation passed.";
        return immunizationsModel;
    }
}
