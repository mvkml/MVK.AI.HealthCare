namespace AI.HealthCare.Patient.Models.Device;

public class DeviceResponse : BaseModel
{
    public long Id { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? Stop { get; set; }
    public Guid PatientId { get; set; }
    public Guid EncounterId { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Udi { get; set; }
}
