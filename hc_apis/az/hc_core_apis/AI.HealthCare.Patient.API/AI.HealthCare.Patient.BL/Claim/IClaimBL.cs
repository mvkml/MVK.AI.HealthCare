using AI.HealthCare.Patient.Models.Claim;
using AI.HealthCare.Patient.Models.Shared;

namespace AI.HealthCare.Patient.BL;

public interface IClaimBL
{
    Task<ClaimsModel> Create(ClaimsModel claimsModel);
    Task<ClaimsModel> GetById(ClaimsModel claimsModel);
    Task<ClaimsModel> GetAll(ClaimsModel claimsModel);
    Task<ClaimsModel> GetByPatientId(Guid patientId);
    Task<ClaimsModel> Update(ClaimsModel claimsModel);
    Task<ClaimsModel> Delete(ClaimsModel claimsModel);
    Task<ImportResult> Import(Stream csvStream);
    Task<ImportResult> ImportUpsert(Stream csvStream);
}
