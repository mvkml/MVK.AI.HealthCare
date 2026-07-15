namespace AI.HealthCare.Patient.EF.Entities;

public class Encounter
{
    public Guid Id { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? Stop { get; set; }
    public Guid PatientId { get; set; }
    public Entities.Patient? Patient { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public Guid ProviderId { get; set; }
    public Provider? Provider { get; set; }
    public Guid? PayerId { get; set; }
    public Payer? Payer { get; set; }
    public string? EncounterClass { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public decimal? BaseEncounterCost { get; set; }
    public decimal? TotalClaimCost { get; set; }
    public decimal? PayerCoverage { get; set; }
    public string? ReasonCode { get; set; }
    public string? ReasonDescription { get; set; }
}
