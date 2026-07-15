using AI.HealthCare.Patient.Models.Claim;
using EfClaim = AI.HealthCare.Patient.EF.Entities.Claim;

namespace AI.HealthCare.Patient.Repositories;

public interface IClaimMapper
{
    ClaimItem ToModel(EfClaim entity);
    EfClaim ToEntity(ClaimItem item);
}
