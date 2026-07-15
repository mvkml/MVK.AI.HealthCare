namespace AI.HealthCare.Patient.EF.Entities;

public class Condition
{
    public long Id { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? Stop { get; set; }
    public Guid PatientId { get; set; }
    public Entities.Patient? Patient { get; set; }
    public Guid EncounterId { get; set; }
    public Encounter? Encounter { get; set; }
    public string? System { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
}
