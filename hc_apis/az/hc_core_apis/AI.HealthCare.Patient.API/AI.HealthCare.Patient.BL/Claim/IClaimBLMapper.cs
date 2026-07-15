using AI.HealthCare.Patient.Models.Claim;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IClaimBLMapper : ICsvRowParser<ClaimItem>
{
    ClaimItem ToItem(ClaimRequest request);
    ClaimResponse ToResponse(ClaimItem item);
}
