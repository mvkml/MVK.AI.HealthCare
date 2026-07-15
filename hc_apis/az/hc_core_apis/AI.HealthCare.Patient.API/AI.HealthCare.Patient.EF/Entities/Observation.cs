namespace AI.HealthCare.Patient.EF.Entities;

public class Observation
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public Guid PatientId { get; set; }
    public Entities.Patient? Patient { get; set; }
    public Guid EncounterId { get; set; }
    public Encounter? Encounter { get; set; }
    public string? Category { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Value { get; set; }
    public string? Units { get; set; }
    public string? Type { get; set; }
}
