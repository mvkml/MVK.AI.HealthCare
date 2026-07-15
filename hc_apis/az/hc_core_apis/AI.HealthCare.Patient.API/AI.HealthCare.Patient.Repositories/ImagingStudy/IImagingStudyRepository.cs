using AI.HealthCare.Patient.Models.ImagingStudy;

namespace AI.HealthCare.Patient.Repositories;

public interface IImagingStudyRepository
{
    Task<ImagingStudyItem?> GetById(Guid id);
    Task<List<ImagingStudyItem>> GetAll();
    Task<List<ImagingStudyItem>> GetByPatientId(Guid patientId);
    Task<ImagingStudyItem> Create(ImagingStudyItem imagingStudyItem);
    Task CreateBatch(List<ImagingStudyItem> imagingStudyItems);
    Task<ImagingStudyItem?> Update(ImagingStudyItem imagingStudyItem);
    Task<bool> Delete(Guid id);
}
