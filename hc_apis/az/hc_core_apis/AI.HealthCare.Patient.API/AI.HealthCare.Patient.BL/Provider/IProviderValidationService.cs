using AI.HealthCare.Patient.Models.Provider;

namespace AI.HealthCare.Patient.BL;

public interface IProviderValidationService
{
    ProvidersModel Validate(ProvidersModel providersModel);
}
