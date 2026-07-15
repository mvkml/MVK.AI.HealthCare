namespace AI.HealthCare.Patient.Models.Medication;

public class MedicationRequest
{
    public DateTime? Start { get; set; }
    public DateTime? Stop { get; set; }
    public Guid PatientId { get; set; }
    public Guid? PayerId { get; set; }
    public Guid EncounterId { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public decimal? BaseCost { get; set; }
    public decimal? PayerCoverage { get; set; }
    public decimal? TotalCost { get; set; }
    public int? Dispenses { get; set; }
    public string? ReasonCode { get; set; }
    public string? ReasonDescription { get; set; }
}
