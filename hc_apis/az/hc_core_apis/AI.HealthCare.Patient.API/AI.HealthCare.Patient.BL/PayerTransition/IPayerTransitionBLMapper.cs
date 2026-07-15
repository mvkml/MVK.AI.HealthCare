using AI.HealthCare.Patient.Models.PayerTransition;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IPayerTransitionBLMapper : ICsvRowParser<PayerTransitionItem>
{
    PayerTransitionItem ToItem(PayerTransitionRequest request);
    PayerTransitionResponse ToResponse(PayerTransitionItem item);
}
