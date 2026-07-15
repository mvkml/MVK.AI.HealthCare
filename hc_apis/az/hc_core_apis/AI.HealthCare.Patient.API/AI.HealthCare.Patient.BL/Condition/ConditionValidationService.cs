using AI.HealthCare.Patient.Models.Condition;

namespace AI.HealthCare.Patient.BL;

public class ConditionValidationService : IConditionValidationService
{
    public ConditionsModel Validate(ConditionsModel conditionsModel)
    {
        var request = conditionsModel.ConditionRequest;

        if (request.PatientId == Guid.Empty)
        {
            conditionsModel.IsNotValid = true;
            conditionsModel.Message = "PatientId is required.";
            return conditionsModel;
        }

        if (request.EncounterId == Guid.Empty)
        {
            conditionsModel.IsNotValid = true;
            conditionsModel.Message = "EncounterId is required.";
            return conditionsModel;
        }

        conditionsModel.IsNotValid = false;
        conditionsModel.Message = "Validation passed.";
        return conditionsModel;
    }
}
