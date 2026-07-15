using AI.HealthCare.Patient.Models.Payer;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IPayerBLMapper : ICsvRowParser<PayerItem>
{
    PayerItem ToItem(PayerRequest request);
    PayerResponse ToResponse(PayerItem item);
}
