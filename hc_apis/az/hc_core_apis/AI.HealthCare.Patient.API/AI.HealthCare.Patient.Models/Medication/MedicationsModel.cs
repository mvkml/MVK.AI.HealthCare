namespace AI.HealthCare.Patient.Models.Medication;

public class MedicationsModel : BaseModel
{
    public MedicationRequest MedicationRequest { get; set; } = new();
    public MedicationItem MedicationItem { get; set; } = new();
    public List<MedicationItem> MedicationItems { get; set; } = new();
    public MedicationResponse MedicationResponse { get; set; } = new();
}
