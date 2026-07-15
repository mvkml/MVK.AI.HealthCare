namespace AI.HealthCare.Patient.Models.Immunization;

public class ImmunizationsModel : BaseModel
{
    public ImmunizationRequest ImmunizationRequest { get; set; } = new();
    public ImmunizationItem ImmunizationItem { get; set; } = new();
    public List<ImmunizationItem> ImmunizationItems { get; set; } = new();
    public ImmunizationResponse ImmunizationResponse { get; set; } = new();
}
