using AI.HealthCare.Patient.Models.ImagingStudy;

namespace AI.HealthCare.Patient.BL;

public class ImagingStudyValidationService : IImagingStudyValidationService
{
    public ImagingStudiesModel Validate(ImagingStudiesModel imagingStudiesModel)
    {
        var request = imagingStudiesModel.ImagingStudyRequest;

        if (request.PatientId == Guid.Empty)
        {
            imagingStudiesModel.IsNotValid = true;
            imagingStudiesModel.Message = "PatientId is required.";
            return imagingStudiesModel;
        }

        if (request.EncounterId == Guid.Empty)
        {
            imagingStudiesModel.IsNotValid = true;
            imagingStudiesModel.Message = "EncounterId is required.";
            return imagingStudiesModel;
        }

        if (request.Date == default)
        {
            imagingStudiesModel.IsNotValid = true;
            imagingStudiesModel.Message = "Date is required.";
            return imagingStudiesModel;
        }

        imagingStudiesModel.IsNotValid = false;
        imagingStudiesModel.Message = "Validation passed.";
        return imagingStudiesModel;
    }
}
