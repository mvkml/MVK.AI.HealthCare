namespace AI.HealthCare.Patient.Models.Condition;

public class ConditionItem
{
    public long Id { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? Stop { get; set; }
    public Guid PatientId { get; set; }
    public Guid EncounterId { get; set; }
    public string? System { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
}
