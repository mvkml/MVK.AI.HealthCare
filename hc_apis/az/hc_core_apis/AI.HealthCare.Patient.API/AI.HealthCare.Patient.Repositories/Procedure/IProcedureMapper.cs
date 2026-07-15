using AI.HealthCare.Patient.Models.Procedure;
using EfProcedure = AI.HealthCare.Patient.EF.Entities.Procedure;

namespace AI.HealthCare.Patient.Repositories;

public interface IProcedureMapper
{
    ProcedureItem ToModel(EfProcedure entity);
    EfProcedure ToEntity(ProcedureItem item);
}
