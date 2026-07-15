using AI.HealthCare.Patient.Models.Immunization;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IImmunizationBLMapper : ICsvRowParser<ImmunizationItem>
{
    ImmunizationItem ToItem(ImmunizationRequest request);
    ImmunizationResponse ToResponse(ImmunizationItem item);
}
