namespace AI.HealthCare.Patient.Models.Encounter;

public class EncounterItem
{
    public Guid Id { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? Stop { get; set; }
    public Guid PatientId { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid? PayerId { get; set; }
    public string? EncounterClass { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public decimal? BaseEncounterCost { get; set; }
    public decimal? TotalClaimCost { get; set; }
    public decimal? PayerCoverage { get; set; }
    public string? ReasonCode { get; set; }
    public string? ReasonDescription { get; set; }
}
