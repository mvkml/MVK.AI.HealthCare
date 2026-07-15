using AI.HealthCare.Patient.Models.Payer;

namespace AI.HealthCare.Patient.BL;

public class PayerValidationService : IPayerValidationService
{
    public PayersModel Validate(PayersModel payersModel)
    {
        var request = payersModel.PayerRequest;

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            payersModel.IsNotValid = true;
            payersModel.Message = "Name is required.";
            return payersModel;
        }

        payersModel.IsNotValid = false;
        payersModel.Message = "Validation passed.";
        return payersModel;
    }
}
