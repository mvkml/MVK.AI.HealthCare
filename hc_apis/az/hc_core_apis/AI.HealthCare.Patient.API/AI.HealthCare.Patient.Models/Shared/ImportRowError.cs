namespace AI.HealthCare.Patient.Models.Shared;

public class ImportRowError
{
    public int RowNumber { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
