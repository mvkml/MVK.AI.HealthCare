using AI.HealthCare.Patient.Models.Supply;

namespace AI.HealthCare.Patient.Repositories;

public interface ISupplyRepository
{
    Task<SupplyItem?> GetById(long id);
    Task<List<SupplyItem>> GetAll();
    Task<List<SupplyItem>> GetByPatientId(Guid patientId);
    Task<SupplyItem> Create(SupplyItem supplyItem);
    Task CreateBatch(List<SupplyItem> supplyItems);
    Task<SupplyItem?> Update(SupplyItem supplyItem);
    Task<bool> Delete(long id);
}
