using AI.HealthCare.Patient.Models.Careplan;

namespace AI.HealthCare.Patient.Repositories;

public interface ICareplanRepository
{
    Task<CareplanItem?> GetById(Guid id);
    Task<List<CareplanItem>> GetAll();
    Task<List<CareplanItem>> GetByPatientId(Guid patientId);
    Task<CareplanItem> Create(CareplanItem careplanItem);
    Task CreateBatch(List<CareplanItem> careplanItems);
    Task UpsertBatch(List<CareplanItem> careplanItems);
    Task<CareplanItem?> Update(CareplanItem careplanItem);
    Task<bool> Delete(Guid id);
}
