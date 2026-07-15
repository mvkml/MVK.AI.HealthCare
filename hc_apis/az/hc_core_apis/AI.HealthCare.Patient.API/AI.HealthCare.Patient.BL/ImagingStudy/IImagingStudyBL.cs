using AI.HealthCare.Patient.Models.ImagingStudy;
using AI.HealthCare.Patient.Models.Shared;

namespace AI.HealthCare.Patient.BL;

public interface IImagingStudyBL
{
    Task<ImagingStudiesModel> Create(ImagingStudiesModel imagingStudiesModel);
    Task<ImagingStudiesModel> GetById(ImagingStudiesModel imagingStudiesModel);
    Task<ImagingStudiesModel> GetAll(ImagingStudiesModel imagingStudiesModel);
    Task<ImagingStudiesModel> GetByPatientId(Guid patientId);
    Task<ImagingStudiesModel> Update(ImagingStudiesModel imagingStudiesModel);
    Task<ImagingStudiesModel> Delete(ImagingStudiesModel imagingStudiesModel);
    Task<ImportResult> Import(Stream csvStream);
    Task<ImportResult> ImportUpsert(Stream csvStream);
}
