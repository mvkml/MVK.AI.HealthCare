namespace AI.HealthCare.Patient.Models.PayerTransition;

public class PayerTransitionsModel : BaseModel
{
    public PayerTransitionRequest PayerTransitionRequest { get; set; } = new();
    public PayerTransitionItem PayerTransitionItem { get; set; } = new();
    public List<PayerTransitionItem> PayerTransitionItems { get; set; } = new();
    public PayerTransitionResponse PayerTransitionResponse { get; set; } = new();
}
