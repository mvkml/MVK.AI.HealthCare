using AI.HealthCare.Patient.Models.ClaimTransaction;

namespace AI.HealthCare.Patient.BL;

public class ClaimTransactionValidationService : IClaimTransactionValidationService
{
    public ClaimTransactionsModel Validate(ClaimTransactionsModel claimTransactionsModel)
    {
        var request = claimTransactionsModel.ClaimTransactionRequest;

        if (request.ClaimId == Guid.Empty)
        {
            claimTransactionsModel.IsNotValid = true;
            claimTransactionsModel.Message = "ClaimId is required.";
            return claimTransactionsModel;
        }

        if (request.PatientId == Guid.Empty)
        {
            claimTransactionsModel.IsNotValid = true;
            claimTransactionsModel.Message = "PatientId is required.";
            return claimTransactionsModel;
        }

        if (request.ProviderId == Guid.Empty)
        {
            claimTransactionsModel.IsNotValid = true;
            claimTransactionsModel.Message = "ProviderId is required.";
            return claimTransactionsModel;
        }

        if (request.SupervisingProviderId == Guid.Empty)
        {
            claimTransactionsModel.IsNotValid = true;
            claimTransactionsModel.Message = "SupervisingProviderId is required.";
            return claimTransactionsModel;
        }

        if (string.IsNullOrWhiteSpace(request.Type))
        {
            claimTransactionsModel.IsNotValid = true;
            claimTransactionsModel.Message = "Type is required.";
            return claimTransactionsModel;
        }

        claimTransactionsModel.IsNotValid = false;
        claimTransactionsModel.Message = "Validation passed.";
        return claimTransactionsModel;
    }
}
