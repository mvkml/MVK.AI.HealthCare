using AI.HealthCare.Patient.Models.Payer;

namespace AI.HealthCare.Patient.BL;

public interface IPayerValidationService
{
    PayersModel Validate(PayersModel payersModel);
}
