using AI.HealthCare.Patient.Models.Claim;

namespace AI.HealthCare.Patient.Repositories;

public interface IClaimRepository
{
    Task<ClaimItem?> GetById(Guid id);
    Task<List<ClaimItem>> GetAll();
    Task<List<ClaimItem>> GetByPatientId(Guid patientId);
    Task<ClaimItem> Create(ClaimItem claimItem);
    Task CreateBatch(List<ClaimItem> claimItems);
    Task UpsertBatch(List<ClaimItem> claimItems);
    Task<ClaimItem?> Update(ClaimItem claimItem);
    Task<bool> Delete(Guid id);
}
