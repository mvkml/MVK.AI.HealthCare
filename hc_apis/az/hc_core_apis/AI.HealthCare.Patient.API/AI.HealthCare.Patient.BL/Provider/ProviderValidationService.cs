using AI.HealthCare.Patient.Models.Provider;

namespace AI.HealthCare.Patient.BL;

public class ProviderValidationService : IProviderValidationService
{
    public ProvidersModel Validate(ProvidersModel providersModel)
    {
        var request = providersModel.ProviderRequest;

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            providersModel.IsNotValid = true;
            providersModel.Message = "Name is required.";
            return providersModel;
        }

        if (request.OrganizationId == Guid.Empty)
        {
            providersModel.IsNotValid = true;
            providersModel.Message = "OrganizationId is required.";
            return providersModel;
        }

        providersModel.IsNotValid = false;
        providersModel.Message = "Validation passed.";
        return providersModel;
    }
}
