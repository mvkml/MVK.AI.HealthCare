namespace AI.HealthCare.Patient.Models.Observation;

public class ObservationRequest
{
    public DateTime Date { get; set; }
    public Guid PatientId { get; set; }
    public Guid? EncounterId { get; set; }
    public string? Category { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Value { get; set; }
    public string? Units { get; set; }
    public string? Type { get; set; }
}
