using AI.HealthCare.Patient.Models.Condition;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IConditionBLMapper : ICsvRowParser<ConditionItem>
{
    ConditionItem ToItem(ConditionRequest request);
    ConditionResponse ToResponse(ConditionItem item);
}
