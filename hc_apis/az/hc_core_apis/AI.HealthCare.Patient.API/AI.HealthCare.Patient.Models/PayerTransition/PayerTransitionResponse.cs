namespace AI.HealthCare.Patient.Models.PayerTransition;

public class PayerTransitionResponse : BaseModel
{
    public long Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid MemberId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid PayerId { get; set; }
    public Guid? SecondaryPayerId { get; set; }
    public string? PlanOwnership { get; set; }
    public string? OwnerName { get; set; }
}
