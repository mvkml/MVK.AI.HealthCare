using AI.HealthCare.Patient.Models.Provider;

namespace AI.HealthCare.Patient.BL;

public interface IProviderBL
{
    Task<ProvidersModel> Create(ProvidersModel providersModel);
    Task<ProvidersModel> GetById(ProvidersModel providersModel);
    Task<ProvidersModel> GetAll(ProvidersModel providersModel);
    Task<ProvidersModel> Update(ProvidersModel providersModel);
    Task<ProvidersModel> Delete(ProvidersModel providersModel);
}
