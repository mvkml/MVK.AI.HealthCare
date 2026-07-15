using AI.HealthCare.Patient.Models.Condition;

namespace AI.HealthCare.Patient.BL;

public interface IConditionBL
{
    Task<ConditionsModel> Create(ConditionsModel conditionsModel);
    Task<ConditionsModel> GetById(ConditionsModel conditionsModel);
    Task<ConditionsModel> GetAll(ConditionsModel conditionsModel);
    Task<ConditionsModel> GetByPatientId(Guid patientId);
    Task<ConditionsModel> Update(ConditionsModel conditionsModel);
    Task<ConditionsModel> Delete(ConditionsModel conditionsModel);
}
