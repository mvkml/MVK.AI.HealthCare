namespace AI.HealthCare.Patient.Models.Supply;

public class SupplyResponse : BaseModel
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public Guid PatientId { get; set; }
    public Guid EncounterId { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int? Quantity { get; set; }
}
