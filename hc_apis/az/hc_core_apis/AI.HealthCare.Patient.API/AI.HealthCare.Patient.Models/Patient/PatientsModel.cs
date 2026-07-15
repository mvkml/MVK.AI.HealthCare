namespace AI.HealthCare.Patient.Models.Patient;

public class PatientsModel : BaseModel
{
    public PatientRequest PatientRequest { get; set; } = new();
    public PatientItem PatientItem { get; set; } = new();
    public List<PatientItem> PatientItems { get; set; } = new();
    public PatientResponse PatientResponse { get; set; } = new();

    /// <summary>When true, GetById/GetAll populate Ssn/Drivers/Passport on the response, masked. Set by the Controller from a query-string flag, never bound from PatientRequest.</summary>
    public bool IncludePii { get; set; }
}
