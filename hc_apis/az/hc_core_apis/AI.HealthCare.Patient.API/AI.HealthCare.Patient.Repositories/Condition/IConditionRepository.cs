using AI.HealthCare.Patient.Models.Condition;

namespace AI.HealthCare.Patient.Repositories;

public interface IConditionRepository
{
    Task<ConditionItem?> GetById(long id);
    Task<List<ConditionItem>> GetAll();
    Task<List<ConditionItem>> GetByPatientId(Guid patientId);
    Task<ConditionItem> Create(ConditionItem conditionItem);
    Task<ConditionItem?> Update(ConditionItem conditionItem);
    Task<bool> Delete(long id);
}
