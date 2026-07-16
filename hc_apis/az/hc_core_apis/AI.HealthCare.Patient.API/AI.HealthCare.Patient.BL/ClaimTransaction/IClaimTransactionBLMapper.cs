using AI.HealthCare.Patient.Models.ClaimTransaction;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IClaimTransactionBLMapper : ICsvRowParser<ClaimTransactionItem>
{
    ClaimTransactionItem ToItem(ClaimTransactionRequest request);
    ClaimTransactionResponse ToResponse(ClaimTransactionItem item);
}
