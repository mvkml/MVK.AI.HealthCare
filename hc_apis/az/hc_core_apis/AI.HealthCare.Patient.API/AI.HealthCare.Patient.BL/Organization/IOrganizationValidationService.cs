using AI.HealthCare.Patient.Models.Organization;

namespace AI.HealthCare.Patient.BL;

public interface IOrganizationValidationService
{
    OrganizationsModel Validate(OrganizationsModel organizationsModel);
}
