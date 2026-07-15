namespace AI.HealthCare.Patient.Models.Careplan;

public class CareplanRequest
{
    public DateTime? Start { get; set; }
    public DateTime? Stop { get; set; }
    public Guid PatientId { get; set; }
    public Guid EncounterId { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? ReasonCode { get; set; }
    public string? ReasonDescription { get; set; }
}
