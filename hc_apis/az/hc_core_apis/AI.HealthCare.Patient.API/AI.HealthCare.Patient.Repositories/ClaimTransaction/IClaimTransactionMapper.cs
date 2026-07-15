using AI.HealthCare.Patient.Models.ClaimTransaction;
using EfClaimTransaction = AI.HealthCare.Patient.EF.Entities.ClaimTransaction;

namespace AI.HealthCare.Patient.Repositories;

public interface IClaimTransactionMapper
{
    ClaimTransactionItem ToModel(EfClaimTransaction entity);
    EfClaimTransaction ToEntity(ClaimTransactionItem item);
}
