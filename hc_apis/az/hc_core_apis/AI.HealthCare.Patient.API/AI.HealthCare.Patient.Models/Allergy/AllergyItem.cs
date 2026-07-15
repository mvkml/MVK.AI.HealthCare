namespace AI.HealthCare.Patient.Models.Allergy;

public class AllergyItem
{
    public long Id { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? Stop { get; set; }
    public Guid PatientId { get; set; }
    public Guid EncounterId { get; set; }
    public string? Code { get; set; }
    public string? System { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Category { get; set; }
    public string? Reaction1 { get; set; }
    public string? Description1 { get; set; }
    public string? Severity1 { get; set; }
    public string? Reaction2 { get; set; }
    public string? Description2 { get; set; }
    public string? Severity2 { get; set; }
}
