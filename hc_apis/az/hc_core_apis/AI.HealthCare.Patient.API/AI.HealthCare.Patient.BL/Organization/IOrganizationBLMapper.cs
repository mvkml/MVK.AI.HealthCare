using AI.HealthCare.Patient.Models.Organization;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IOrganizationBLMapper : ICsvRowParser<OrganizationItem>
{
    OrganizationItem ToItem(OrganizationRequest request);
    OrganizationResponse ToResponse(OrganizationItem item);
}
