namespace AI.HealthCare.Patient.Models.Claim;

public class ClaimItem
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid? PrimaryPatientInsuranceId { get; set; }
    public Guid? SecondaryPatientInsuranceId { get; set; }
    public int? DepartmentId { get; set; }
    public int? PatientDepartmentId { get; set; }
    public string? Diagnosis1 { get; set; }
    public string? Diagnosis2 { get; set; }
    public string? Diagnosis3 { get; set; }
    public string? Diagnosis4 { get; set; }
    public string? Diagnosis5 { get; set; }
    public string? Diagnosis6 { get; set; }
    public string? Diagnosis7 { get; set; }
    public string? Diagnosis8 { get; set; }
    public Guid? ReferringProviderId { get; set; }
    public Guid? SupervisingProviderId { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateTime? CurrentIllnessDate { get; set; }
    public DateTime? ServiceDate { get; set; }
    public string? Status1 { get; set; }
    public string? Status2 { get; set; }
    public string? StatusP { get; set; }
    public decimal? Outstanding1 { get; set; }
    public decimal? Outstanding2 { get; set; }
    public decimal? OutstandingP { get; set; }
    public DateTime? LastBilledDate1 { get; set; }
    public DateTime? LastBilledDate2 { get; set; }
    public DateTime? LastBilledDateP { get; set; }
    public int? HealthcareClaimTypeId1 { get; set; }
    public int? HealthcareClaimTypeId2 { get; set; }
}
