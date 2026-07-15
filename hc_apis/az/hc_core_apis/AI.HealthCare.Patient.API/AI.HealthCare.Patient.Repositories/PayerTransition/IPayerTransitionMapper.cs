using AI.HealthCare.Patient.Models.PayerTransition;
using EfPayerTransition = AI.HealthCare.Patient.EF.Entities.PayerTransition;

namespace AI.HealthCare.Patient.Repositories;

public interface IPayerTransitionMapper
{
    PayerTransitionItem ToModel(EfPayerTransition entity);
    EfPayerTransition ToEntity(PayerTransitionItem item);
}
