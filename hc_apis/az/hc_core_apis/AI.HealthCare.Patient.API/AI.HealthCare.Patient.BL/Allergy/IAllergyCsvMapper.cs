using AI.HealthCare.Patient.Models.Allergy;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IAllergyCsvMapper : ICsvRowParser<AllergyItem>
{
    AllergyItem ToItem(AllergyRequest request);
    AllergyResponse ToResponse(AllergyItem item);
}
