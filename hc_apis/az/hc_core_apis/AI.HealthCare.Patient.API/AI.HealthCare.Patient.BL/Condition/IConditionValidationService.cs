using AI.HealthCare.Patient.Models.Condition;

namespace AI.HealthCare.Patient.BL;

public interface IConditionValidationService
{
    ConditionsModel Validate(ConditionsModel conditionsModel);
}
