namespace AI.HealthCare.Patient.Models.Organization;

public class OrganizationItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Phone { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lon { get; set; }
    public decimal? Revenue { get; set; }
    public int? Utilization { get; set; }
}
