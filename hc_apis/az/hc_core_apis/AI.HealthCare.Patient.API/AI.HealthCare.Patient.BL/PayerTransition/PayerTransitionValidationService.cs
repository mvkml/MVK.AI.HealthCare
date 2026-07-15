using AI.HealthCare.Patient.Models.PayerTransition;

namespace AI.HealthCare.Patient.BL;

public class PayerTransitionValidationService : IPayerTransitionValidationService
{
    public PayerTransitionsModel Validate(PayerTransitionsModel payerTransitionsModel)
    {
        var request = payerTransitionsModel.PayerTransitionRequest;

        if (request.PatientId == Guid.Empty)
        {
            payerTransitionsModel.IsNotValid = true;
            payerTransitionsModel.Message = "PatientId is required.";
            return payerTransitionsModel;
        }

        if (request.MemberId == Guid.Empty)
        {
            payerTransitionsModel.IsNotValid = true;
            payerTransitionsModel.Message = "MemberId is required.";
            return payerTransitionsModel;
        }

        if (request.PayerId == Guid.Empty)
        {
            payerTransitionsModel.IsNotValid = true;
            payerTransitionsModel.Message = "PayerId is required.";
            return payerTransitionsModel;
        }

        payerTransitionsModel.IsNotValid = false;
        payerTransitionsModel.Message = "Validation passed.";
        return payerTransitionsModel;
    }
}
