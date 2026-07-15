using AI.HealthCare.Patient.Models.ImagingStudy;

namespace AI.HealthCare.Patient.BL;

public interface IImagingStudyBL
{
    Task<ImagingStudiesModel> Create(ImagingStudiesModel imagingStudiesModel);
    Task<ImagingStudiesModel> GetById(ImagingStudiesModel imagingStudiesModel);
    Task<ImagingStudiesModel> GetAll(ImagingStudiesModel imagingStudiesModel);
    Task<ImagingStudiesModel> GetByPatientId(Guid patientId);
    Task<ImagingStudiesModel> Update(ImagingStudiesModel imagingStudiesModel);
    Task<ImagingStudiesModel> Delete(ImagingStudiesModel imagingStudiesModel);
}
