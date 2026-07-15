using AI.HealthCare.Patient.Models.ImagingStudy;

namespace AI.HealthCare.Patient.BL;

public interface IImagingStudyValidationService
{
    ImagingStudiesModel Validate(ImagingStudiesModel imagingStudiesModel);
}
