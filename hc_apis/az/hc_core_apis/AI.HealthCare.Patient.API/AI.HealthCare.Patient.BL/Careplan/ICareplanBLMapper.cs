using AI.HealthCare.Patient.Models.Careplan;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface ICareplanBLMapper : ICsvRowParser<CareplanItem>
{
    CareplanItem ToItem(CareplanRequest request);
    CareplanResponse ToResponse(CareplanItem item);
}
