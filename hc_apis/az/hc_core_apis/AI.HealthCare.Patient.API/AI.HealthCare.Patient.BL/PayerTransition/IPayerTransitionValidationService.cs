using AI.HealthCare.Patient.Models.PayerTransition;

namespace AI.HealthCare.Patient.BL;

public interface IPayerTransitionValidationService
{
    PayerTransitionsModel Validate(PayerTransitionsModel payerTransitionsModel);
}
