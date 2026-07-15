using AI.HealthCare.Patient.Models.Procedure;
using AI.HealthCare.Patient.Models.Shared;

namespace AI.HealthCare.Patient.BL;

public interface IProcedureBL
{
    Task<ProceduresModel> Create(ProceduresModel proceduresModel);
    Task<ProceduresModel> GetById(ProceduresModel proceduresModel);
    Task<ProceduresModel> GetAll(ProceduresModel proceduresModel);
    Task<ProceduresModel> GetByPatientId(Guid patientId);
    Task<ProceduresModel> Update(ProceduresModel proceduresModel);
    Task<ProceduresModel> Delete(ProceduresModel proceduresModel);
    Task<ImportResult> Import(Stream csvStream);
    Task<ImportResult> ImportUpsert(Stream csvStream);
}
