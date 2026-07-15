using AI.HealthCare.Patient.Models.Organization;

namespace AI.HealthCare.Patient.Repositories;

public interface IOrganizationRepository
{
    Task<OrganizationItem?> GetById(Guid id);
    Task<List<OrganizationItem>> GetAll();
    Task<OrganizationItem> Create(OrganizationItem organizationItem);
    Task<OrganizationItem?> Update(OrganizationItem organizationItem);
    Task<bool> Delete(Guid id);
}
