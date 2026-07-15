namespace AI.HealthCare.Patient.Models.Shared;

public class ImportResult
{
    public int TotalRows { get; set; }
    public int InsertedCount { get; set; }
    public int FailedCount { get; set; }
    public List<ImportRowError> Errors { get; set; } = new();
}
