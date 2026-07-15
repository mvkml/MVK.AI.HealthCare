using AI.HealthCare.Patient.Models.Condition;
using AI.HealthCare.Patient.Models.Shared;

namespace AI.HealthCare.Patient.BL;

public interface IConditionBL
{
    Task<ConditionsModel> Create(ConditionsModel conditionsModel);
    Task<ConditionsModel> GetById(ConditionsModel conditionsModel);
    Task<ConditionsModel> GetAll(ConditionsModel conditionsModel);
    Task<ConditionsModel> GetByPatientId(Guid patientId);
    Task<ConditionsModel> Update(ConditionsModel conditionsModel);
    Task<ConditionsModel> Delete(ConditionsModel conditionsModel);
    Task<ImportResult> Import(Stream csvStream);
    Task<ImportResult> ImportUpsert(Stream csvStream);
}
