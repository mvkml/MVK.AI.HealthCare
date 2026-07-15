using AI.HealthCare.Patient.Models.ClaimTransaction;

namespace AI.HealthCare.Patient.BL;

public interface IClaimTransactionValidationService
{
    ClaimTransactionsModel Validate(ClaimTransactionsModel claimTransactionsModel);
}
