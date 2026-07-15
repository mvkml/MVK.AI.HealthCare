using AI.HealthCare.Patient.Models.PayerTransition;

namespace AI.HealthCare.Patient.Repositories;

public interface IPayerTransitionRepository
{
    Task<PayerTransitionItem?> GetById(long id);
    Task<List<PayerTransitionItem>> GetAll();
    Task<PayerTransitionItem> Create(PayerTransitionItem payerTransitionItem);
    Task CreateBatch(List<PayerTransitionItem> payerTransitionItems);
    Task<PayerTransitionItem?> Update(PayerTransitionItem payerTransitionItem);
    Task<bool> Delete(long id);
}
