namespace AI.HealthCare.Patient.Models.Condition;

public class ConditionsModel : BaseModel
{
    public ConditionRequest ConditionRequest { get; set; } = new();
    public ConditionItem ConditionItem { get; set; } = new();
    public List<ConditionItem> ConditionItems { get; set; } = new();
    public ConditionResponse ConditionResponse { get; set; } = new();
}
