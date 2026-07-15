using AI.HealthCare.Patient.Models.Claim;

namespace AI.HealthCare.Patient.BL;

public class ClaimValidationService : IClaimValidationService
{
    public ClaimsModel Validate(ClaimsModel claimsModel)
    {
        var request = claimsModel.ClaimRequest;

        if (request.PatientId == Guid.Empty)
        {
            claimsModel.IsNotValid = true;
            claimsModel.Message = "PatientId is required.";
            return claimsModel;
        }

        if (request.ProviderId == Guid.Empty)
        {
            claimsModel.IsNotValid = true;
            claimsModel.Message = "ProviderId is required.";
            return claimsModel;
        }

        claimsModel.IsNotValid = false;
        claimsModel.Message = "Validation passed.";
        return claimsModel;
    }
}
