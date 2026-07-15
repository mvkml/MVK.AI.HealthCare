namespace AI.HealthCare.Patient.EF.Entities;

public class Provider
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string? Speciality { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lon { get; set; }
    public int? Encounters { get; set; }
    public int? Procedures { get; set; }
}
