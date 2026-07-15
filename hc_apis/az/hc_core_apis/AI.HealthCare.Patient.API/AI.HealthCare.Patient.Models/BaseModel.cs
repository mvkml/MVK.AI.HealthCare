namespace AI.HealthCare.Patient.Models;

public class BaseModel
{
    public bool IsNotValid { get; set; }
    public string Message { get; set; } = string.Empty;
}
