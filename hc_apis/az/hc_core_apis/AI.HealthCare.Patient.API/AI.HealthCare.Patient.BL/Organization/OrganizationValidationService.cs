using AI.HealthCare.Patient.Models.Organization;

namespace AI.HealthCare.Patient.BL;

public class OrganizationValidationService : IOrganizationValidationService
{
    public OrganizationsModel Validate(OrganizationsModel organizationsModel)
    {
        var request = organizationsModel.OrganizationRequest;

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            organizationsModel.IsNotValid = true;
            organizationsModel.Message = "Name is required.";
            return organizationsModel;
        }

        organizationsModel.IsNotValid = false;
        organizationsModel.Message = "Validation passed.";
        return organizationsModel;
    }
}
