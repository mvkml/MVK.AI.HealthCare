namespace AI.HealthCare.Patient.Models.Payer;

public class PayersModel : BaseModel
{
    public PayerRequest PayerRequest { get; set; } = new();
    public PayerItem PayerItem { get; set; } = new();
    public List<PayerItem> PayerItems { get; set; } = new();
    public PayerResponse PayerResponse { get; set; } = new();
}
