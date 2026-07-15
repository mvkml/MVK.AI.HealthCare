using AI.HealthCare.Patient.Models.Provider;
using EfProvider = AI.HealthCare.Patient.EF.Entities.Provider;

namespace AI.HealthCare.Patient.Repositories;

public interface IProviderMapper
{
    ProviderItem ToModel(EfProvider entity);
    EfProvider ToEntity(ProviderItem item);
}
