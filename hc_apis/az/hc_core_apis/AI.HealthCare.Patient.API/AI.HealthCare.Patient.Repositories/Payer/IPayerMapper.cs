using AI.HealthCare.Patient.Models.Payer;
using EfPayer = AI.HealthCare.Patient.EF.Entities.Payer;

namespace AI.HealthCare.Patient.Repositories;

public interface IPayerMapper
{
    PayerItem ToModel(EfPayer entity);
    EfPayer ToEntity(PayerItem item);
}
