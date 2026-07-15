namespace AI.HealthCare.Patient.Models.Patient;

public class PatientRequest
{
    public DateTime BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string? Ssn { get; set; }
    public string? Drivers { get; set; }
    public string? Passport { get; set; }
    public string? Prefix { get; set; }
    public string First { get; set; } = string.Empty;
    public string? Middle { get; set; }
    public string Last { get; set; } = string.Empty;
    public string? Suffix { get; set; }
    public string? Maiden { get; set; }
    public string? Marital { get; set; }
    public string? Race { get; set; }
    public string? Ethnicity { get; set; }
    public string? Gender { get; set; }
    public string? Birthplace { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? County { get; set; }
    public string? Fips { get; set; }
    public string? Zip { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lon { get; set; }
    public decimal? HealthcareExpenses { get; set; }
    public decimal? HealthcareCoverage { get; set; }
    public int? Income { get; set; }
}
