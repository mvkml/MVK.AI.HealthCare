namespace AI.HealthCare.Patient.Models.Encounter;

public class EncountersModel : BaseModel
{
    public EncounterRequest EncounterRequest { get; set; } = new();
    public EncounterItem EncounterItem { get; set; } = new();
    public List<EncounterItem> EncounterItems { get; set; } = new();
    public EncounterResponse EncounterResponse { get; set; } = new();
}
