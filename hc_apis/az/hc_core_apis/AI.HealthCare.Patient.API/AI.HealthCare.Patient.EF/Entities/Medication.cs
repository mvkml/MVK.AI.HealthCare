namespace AI.HealthCare.Patient.EF.Entities;

public class Medication
{
    public long Id { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? Stop { get; set; }
    public Guid PatientId { get; set; }
    public Entities.Patient? Patient { get; set; }
    public Guid? PayerId { get; set; }
    public Payer? Payer { get; set; }
    public Guid EncounterId { get; set; }
    public Encounter? Encounter { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public decimal? BaseCost { get; set; }
    public decimal? PayerCoverage { get; set; }
    public decimal? TotalCost { get; set; }
    public int? Dispenses { get; set; }
    public string? ReasonCode { get; set; }
    public string? ReasonDescription { get; set; }
}
