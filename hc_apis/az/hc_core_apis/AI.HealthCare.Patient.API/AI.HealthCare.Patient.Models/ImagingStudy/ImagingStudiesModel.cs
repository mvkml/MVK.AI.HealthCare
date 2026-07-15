namespace AI.HealthCare.Patient.Models.ImagingStudy;

public class ImagingStudiesModel : BaseModel
{
    public ImagingStudyRequest ImagingStudyRequest { get; set; } = new();
    public ImagingStudyItem ImagingStudyItem { get; set; } = new();
    public List<ImagingStudyItem> ImagingStudyItems { get; set; } = new();
    public ImagingStudyResponse ImagingStudyResponse { get; set; } = new();
}
