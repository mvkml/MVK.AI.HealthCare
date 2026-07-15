using AI.HealthCare.Patient.Models.Provider;

namespace AI.HealthCare.Patient.Repositories;

public interface IProviderRepository
{
    Task<ProviderItem?> GetById(Guid id);
    Task<List<ProviderItem>> GetAll();
    Task<ProviderItem> Create(ProviderItem providerItem);
    Task CreateBatch(List<ProviderItem> providerItems);
    Task<ProviderItem?> Update(ProviderItem providerItem);
    Task<bool> Delete(Guid id);
}
