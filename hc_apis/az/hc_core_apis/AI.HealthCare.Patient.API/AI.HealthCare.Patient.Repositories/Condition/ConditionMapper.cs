using AI.HealthCare.Patient.Models.Condition;
using EfCondition = AI.HealthCare.Patient.EF.Entities.Condition;

namespace AI.HealthCare.Patient.Repositories;

public class ConditionMapper : IConditionMapper
{
    public ConditionItem ToModel(EfCondition entity) => new()
    {
        Id = entity.Id,
        Start = entity.Start,
        Stop = entity.Stop,
        PatientId = entity.PatientId,
        EncounterId = entity.EncounterId,
        System = entity.System,
        Code = entity.Code,
        Description = entity.Description
    };

    public EfCondition ToEntity(ConditionItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        System = item.System,
        Code = item.Code,
        Description = item.Description
    };
}
