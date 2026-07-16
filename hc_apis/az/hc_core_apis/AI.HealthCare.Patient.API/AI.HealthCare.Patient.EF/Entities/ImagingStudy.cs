namespace AI.HealthCare.Patient.EF.Entities;

public class ImagingStudy
{
    public long Id { get; set; }
    /// <summary>Synthea's study Id. Not unique per row — one study can span many series/instance rows.</summary>
    public Guid StudyId { get; set; }
    public DateTime Date { get; set; }
    public Guid PatientId { get; set; }
    public Entities.Patient? Patient { get; set; }
    public Guid EncounterId { get; set; }
    public Encounter? Encounter { get; set; }
    public string? SeriesUid { get; set; }
    public string? BodysiteCode { get; set; }
    public string? BodysiteDescription { get; set; }
    public string? ModalityCode { get; set; }
    public string? ModalityDescription { get; set; }
    public string? InstanceUid { get; set; }
    public string? SopCode { get; set; }
    public string? SopDescription { get; set; }
    public string? ProcedureCode { get; set; }
}
