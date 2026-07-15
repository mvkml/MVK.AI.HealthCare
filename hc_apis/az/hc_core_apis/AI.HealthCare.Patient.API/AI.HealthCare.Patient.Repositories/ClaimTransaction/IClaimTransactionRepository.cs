using AI.HealthCare.Patient.Models.ClaimTransaction;

namespace AI.HealthCare.Patient.Repositories;

public interface IClaimTransactionRepository
{
    Task<ClaimTransactionItem?> GetById(Guid id);
    Task<List<ClaimTransactionItem>> GetAll();
    Task<List<ClaimTransactionItem>> GetByClaimId(Guid claimId);
    Task<ClaimTransactionItem> Create(ClaimTransactionItem claimTransactionItem);
    Task<ClaimTransactionItem?> Update(ClaimTransactionItem claimTransactionItem);
    Task<bool> Delete(Guid id);
}
