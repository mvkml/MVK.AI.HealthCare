namespace AI.HealthCare.Patient.Models.Observation;

public class ObservationsModel : BaseModel
{
    public ObservationRequest ObservationRequest { get; set; } = new();
    public ObservationItem ObservationItem { get; set; } = new();
    public List<ObservationItem> ObservationItems { get; set; } = new();
    public ObservationResponse ObservationResponse { get; set; } = new();
}
