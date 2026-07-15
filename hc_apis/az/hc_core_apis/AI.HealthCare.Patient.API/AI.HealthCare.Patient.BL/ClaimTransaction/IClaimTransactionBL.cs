using AI.HealthCare.Patient.Models.ClaimTransaction;

namespace AI.HealthCare.Patient.BL;

public interface IClaimTransactionBL
{
    Task<ClaimTransactionsModel> Create(ClaimTransactionsModel claimTransactionsModel);
    Task<ClaimTransactionsModel> GetById(ClaimTransactionsModel claimTransactionsModel);
    Task<ClaimTransactionsModel> GetAll(ClaimTransactionsModel claimTransactionsModel);
    Task<ClaimTransactionsModel> GetByClaimId(Guid claimId);
    Task<ClaimTransactionsModel> Update(ClaimTransactionsModel claimTransactionsModel);
    Task<ClaimTransactionsModel> Delete(ClaimTransactionsModel claimTransactionsModel);
}
