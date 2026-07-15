namespace AI.HealthCare.Patient.Models.Careplan;

public class CareplansModel : BaseModel
{
    public CareplanRequest CareplanRequest { get; set; } = new();
    public CareplanItem CareplanItem { get; set; } = new();
    public List<CareplanItem> CareplanItems { get; set; } = new();
    public CareplanResponse CareplanResponse { get; set; } = new();
}
