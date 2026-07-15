using AI.HealthCare.Patient.Models.Organization;
using AI.HealthCare.Patient.Models.Shared;

namespace AI.HealthCare.Patient.BL;

public interface IOrganizationBL
{
    Task<OrganizationsModel> Create(OrganizationsModel organizationsModel);
    Task<OrganizationsModel> GetById(OrganizationsModel organizationsModel);
    Task<OrganizationsModel> GetAll(OrganizationsModel organizationsModel);
    Task<OrganizationsModel> Update(OrganizationsModel organizationsModel);
    Task<OrganizationsModel> Delete(OrganizationsModel organizationsModel);
    Task<ImportResult> Import(Stream csvStream);
}
