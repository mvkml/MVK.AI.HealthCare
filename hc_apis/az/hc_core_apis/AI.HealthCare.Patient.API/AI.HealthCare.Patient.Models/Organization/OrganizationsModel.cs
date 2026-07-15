namespace AI.HealthCare.Patient.Models.Organization;

public class OrganizationsModel : BaseModel
{
    public OrganizationRequest OrganizationRequest { get; set; } = new();
    public OrganizationItem OrganizationItem { get; set; } = new();
    public List<OrganizationItem> OrganizationItems { get; set; } = new();
    public OrganizationResponse OrganizationResponse { get; set; } = new();
}
