namespace AI.HealthCare.Patient.EF.Entities;

public class PayerTransition
{
    public long Id { get; set; }
    public Guid PatientId { get; set; }
    public Entities.Patient? Patient { get; set; }
    public Guid MemberId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid PayerId { get; set; }
    public Payer? Payer { get; set; }
    public Guid? SecondaryPayerId { get; set; }
    public Payer? SecondaryPayer { get; set; }
    public string? PlanOwnership { get; set; }
    public string? OwnerName { get; set; }
}
