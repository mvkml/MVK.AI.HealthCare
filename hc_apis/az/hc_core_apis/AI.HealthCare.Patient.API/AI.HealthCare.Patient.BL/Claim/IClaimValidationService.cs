using AI.HealthCare.Patient.Models.Claim;

namespace AI.HealthCare.Patient.BL;

public interface IClaimValidationService
{
    ClaimsModel Validate(ClaimsModel claimsModel);
}
