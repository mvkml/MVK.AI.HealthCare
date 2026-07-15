using AI.HealthCare.Patient.Models.ImagingStudy;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IImagingStudyBLMapper : ICsvRowParser<ImagingStudyItem>
{
    ImagingStudyItem ToItem(ImagingStudyRequest request);
    ImagingStudyResponse ToResponse(ImagingStudyItem item);
}
