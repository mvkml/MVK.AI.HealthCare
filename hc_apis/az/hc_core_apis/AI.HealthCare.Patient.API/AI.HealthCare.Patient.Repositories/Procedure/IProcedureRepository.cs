using AI.HealthCare.Patient.Models.Procedure;

namespace AI.HealthCare.Patient.Repositories;

public interface IProcedureRepository
{
    Task<ProcedureItem?> GetById(long id);
    Task<List<ProcedureItem>> GetAll();
    Task<List<ProcedureItem>> GetByPatientId(Guid patientId);
    Task<ProcedureItem> Create(ProcedureItem procedureItem);
    Task CreateBatch(List<ProcedureItem> procedureItems);
    Task UpsertBatch(List<ProcedureItem> procedureItems);
    Task<ProcedureItem?> Update(ProcedureItem procedureItem);
    Task<bool> Delete(long id);
}
