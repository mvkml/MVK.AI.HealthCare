using AI.HealthCare.Patient.Models.Payer;

namespace AI.HealthCare.Patient.Repositories;

public interface IPayerRepository
{
    Task<PayerItem?> GetById(Guid id);
    Task<List<PayerItem>> GetAll();
    Task<PayerItem> Create(PayerItem payerItem);
    Task CreateBatch(List<PayerItem> payerItems);
    Task<PayerItem?> Update(PayerItem payerItem);
    Task<bool> Delete(Guid id);
}
