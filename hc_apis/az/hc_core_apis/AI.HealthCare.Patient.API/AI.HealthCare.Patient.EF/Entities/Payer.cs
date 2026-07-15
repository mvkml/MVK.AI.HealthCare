namespace AI.HealthCare.Patient.EF.Entities;

public class Payer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Ownership { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? StateHeadquartered { get; set; }
    public string? Zip { get; set; }
    public string? Phone { get; set; }
    public decimal? AmountCovered { get; set; }
    public decimal? AmountUncovered { get; set; }
    public decimal? Revenue { get; set; }
    public int? CoveredEncounters { get; set; }
    public int? UncoveredEncounters { get; set; }
    public int? CoveredMedications { get; set; }
    public int? UncoveredMedications { get; set; }
    public int? CoveredProcedures { get; set; }
    public int? UncoveredProcedures { get; set; }
    public int? CoveredImmunizations { get; set; }
    public int? UncoveredImmunizations { get; set; }
    public int? UniqueCustomers { get; set; }
    public decimal? QolsAvg { get; set; }
    public int? MemberMonths { get; set; }
}
