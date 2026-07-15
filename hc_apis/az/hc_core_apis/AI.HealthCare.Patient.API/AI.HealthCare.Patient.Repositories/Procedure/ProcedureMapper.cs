using AI.HealthCare.Patient.Models.Procedure;
using EfProcedure = AI.HealthCare.Patient.EF.Entities.Procedure;

namespace AI.HealthCare.Patient.Repositories;

public class ProcedureMapper : IProcedureMapper
{
    public ProcedureItem ToModel(EfProcedure entity) => new()
    {
        Id = entity.Id,
        Start = entity.Start,
        Stop = entity.Stop,
        PatientId = entity.PatientId,
        EncounterId = entity.EncounterId,
        System = entity.System,
        Code = entity.Code,
        Description = entity.Description,
        BaseCost = entity.BaseCost,
        ReasonCode = entity.ReasonCode,
        ReasonDescription = entity.ReasonDescription
    };

    public EfProcedure ToEntity(ProcedureItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        System = item.System,
        Code = item.Code,
        Description = item.Description,
        BaseCost = item.BaseCost,
        ReasonCode = item.ReasonCode,
        ReasonDescription = item.ReasonDescription
    };
}
