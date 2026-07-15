using AI.HealthCare.Patient.Models.ImagingStudy;
using EfImagingStudy = AI.HealthCare.Patient.EF.Entities.ImagingStudy;

namespace AI.HealthCare.Patient.Repositories;

public interface IImagingStudyMapper
{
    ImagingStudyItem ToModel(EfImagingStudy entity);
    EfImagingStudy ToEntity(ImagingStudyItem item);
}
