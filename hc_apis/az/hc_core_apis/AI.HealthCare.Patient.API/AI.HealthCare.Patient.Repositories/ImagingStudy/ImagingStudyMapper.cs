using AI.HealthCare.Patient.Models.ImagingStudy;
using EfImagingStudy = AI.HealthCare.Patient.EF.Entities.ImagingStudy;

namespace AI.HealthCare.Patient.Repositories;

public class ImagingStudyMapper : IImagingStudyMapper
{
    public ImagingStudyItem ToModel(EfImagingStudy entity) => new()
    {
        Id = entity.Id,
        StudyId = entity.StudyId,
        Date = entity.Date,
        PatientId = entity.PatientId,
        EncounterId = entity.EncounterId,
        SeriesUid = entity.SeriesUid,
        BodysiteCode = entity.BodysiteCode,
        BodysiteDescription = entity.BodysiteDescription,
        ModalityCode = entity.ModalityCode,
        ModalityDescription = entity.ModalityDescription,
        InstanceUid = entity.InstanceUid,
        SopCode = entity.SopCode,
        SopDescription = entity.SopDescription,
        ProcedureCode = entity.ProcedureCode
    };

    public EfImagingStudy ToEntity(ImagingStudyItem item) => new()
    {
        Id = item.Id,
        StudyId = item.StudyId,
        Date = item.Date,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        SeriesUid = item.SeriesUid,
        BodysiteCode = item.BodysiteCode,
        BodysiteDescription = item.BodysiteDescription,
        ModalityCode = item.ModalityCode,
        ModalityDescription = item.ModalityDescription,
        InstanceUid = item.InstanceUid,
        SopCode = item.SopCode,
        SopDescription = item.SopDescription,
        ProcedureCode = item.ProcedureCode
    };
}
