namespace AI.HealthCare.Patient.Models.Supply;

public class SuppliesModel : BaseModel
{
    public SupplyRequest SupplyRequest { get; set; } = new();
    public SupplyItem SupplyItem { get; set; } = new();
    public List<SupplyItem> SupplyItems { get; set; } = new();
    public SupplyResponse SupplyResponse { get; set; } = new();
}
