namespace AI.HealthCare.Patient.EF.Entities;

public class ClaimTransaction
{
    public Guid Id { get; set; }
    public Guid ClaimId { get; set; }
    public Claim? Claim { get; set; }
    public int ChargeId { get; set; }
    public Guid PatientId { get; set; }
    public Entities.Patient? Patient { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Method { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid? PlaceOfServiceId { get; set; }
    public Organization? PlaceOfService { get; set; }
    public string? ProcedureCode { get; set; }
    public string? Modifier1 { get; set; }
    public string? Modifier2 { get; set; }
    public int? DiagnosisRef1 { get; set; }
    public int? DiagnosisRef2 { get; set; }
    public int? DiagnosisRef3 { get; set; }
    public int? DiagnosisRef4 { get; set; }
    public int Units { get; set; }
    public int? DepartmentId { get; set; }
    public string? Notes { get; set; }
    public decimal UnitAmount { get; set; }
    public string? TransferOutId { get; set; }
    public string? TransferType { get; set; }
    public decimal? Payments { get; set; }
    public decimal? Adjustments { get; set; }
    public decimal? Transfers { get; set; }
    public decimal? Outstanding { get; set; }
    public Guid? AppointmentId { get; set; }
    public Encounter? Appointment { get; set; }
    public string? LineNote { get; set; }
    public int? PatientInsuranceId { get; set; }
    public Guid? FeeScheduleId { get; set; }
    public Guid ProviderId { get; set; }
    public Provider? Provider { get; set; }
    public Guid SupervisingProviderId { get; set; }
    public Provider? SupervisingProvider { get; set; }
}
