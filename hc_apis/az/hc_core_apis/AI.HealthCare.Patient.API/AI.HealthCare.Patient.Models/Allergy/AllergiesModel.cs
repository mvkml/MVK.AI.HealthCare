namespace AI.HealthCare.Patient.Models.Allergy;

public class AllergiesModel : BaseModel
{
    public AllergyRequest AllergyRequest { get; set; } = new();
    public AllergyItem AllergyItem { get; set; } = new();
    public List<AllergyItem> AllergyItems { get; set; } = new();
    public AllergyResponse AllergyResponse { get; set; } = new();
}
