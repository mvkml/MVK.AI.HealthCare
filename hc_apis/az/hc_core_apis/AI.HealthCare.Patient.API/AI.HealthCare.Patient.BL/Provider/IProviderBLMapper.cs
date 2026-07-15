using AI.HealthCare.Patient.Models.Provider;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IProviderBLMapper : ICsvRowParser<ProviderItem>
{
    ProviderItem ToItem(ProviderRequest request);
    ProviderResponse ToResponse(ProviderItem item);
}
