using AI.HealthCare.Patient.Models.Immunization;

namespace AI.HealthCare.Patient.BL;

public interface IImmunizationValidationService
{
    ImmunizationsModel Validate(ImmunizationsModel immunizationsModel);
}
