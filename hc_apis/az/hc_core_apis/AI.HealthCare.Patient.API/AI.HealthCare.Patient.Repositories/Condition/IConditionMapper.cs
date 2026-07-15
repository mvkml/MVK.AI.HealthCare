using AI.HealthCare.Patient.Models.Condition;
using EfCondition = AI.HealthCare.Patient.EF.Entities.Condition;

namespace AI.HealthCare.Patient.Repositories;

public interface IConditionMapper
{
    ConditionItem ToModel(EfCondition entity);
    EfCondition ToEntity(ConditionItem item);
}
