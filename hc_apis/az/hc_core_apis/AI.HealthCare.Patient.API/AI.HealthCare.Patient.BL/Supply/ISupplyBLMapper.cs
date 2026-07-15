using AI.HealthCare.Patient.Models.Supply;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface ISupplyBLMapper : ICsvRowParser<SupplyItem>
{
    SupplyItem ToItem(SupplyRequest request);
    SupplyResponse ToResponse(SupplyItem item);
}
