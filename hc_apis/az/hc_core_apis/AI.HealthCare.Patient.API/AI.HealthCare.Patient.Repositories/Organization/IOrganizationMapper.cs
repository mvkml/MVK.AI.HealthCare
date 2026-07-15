using AI.HealthCare.Patient.Models.Organization;
using EfOrganization = AI.HealthCare.Patient.EF.Entities.Organization;

namespace AI.HealthCare.Patient.Repositories;

public interface IOrganizationMapper
{
    OrganizationItem ToModel(EfOrganization entity);
    EfOrganization ToEntity(OrganizationItem item);
}
