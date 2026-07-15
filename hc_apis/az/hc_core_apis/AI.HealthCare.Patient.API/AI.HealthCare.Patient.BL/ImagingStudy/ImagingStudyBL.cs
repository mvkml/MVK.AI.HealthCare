using AI.HealthCare.Patient.Models.ImagingStudy;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ImagingStudyBL : IImagingStudyBL
{
    private readonly IImagingStudyRepository _imagingStudyRepository;

    public ImagingStudyBL(IImagingStudyRepository imagingStudyRepository)
    {
        _imagingStudyRepository = imagingStudyRepository;
    }

    public async Task<ImagingStudiesModel> Create(ImagingStudiesModel imagingStudiesModel)
    {
        imagingStudiesModel.ImagingStudyItem = ToItem(imagingStudiesModel.ImagingStudyRequest);
        imagingStudiesModel.ImagingStudyItem.Id = Guid.NewGuid();

        var savedItem = await _imagingStudyRepository.Create(imagingStudiesModel.ImagingStudyItem);
        imagingStudiesModel.ImagingStudyItem = savedItem;

        imagingStudiesModel.ImagingStudyResponse = ToResponse(savedItem);
        imagingStudiesModel.IsNotValid = false;
        imagingStudiesModel.Message = "ImagingStudy created successfully.";

        return imagingStudiesModel;
    }

    public async Task<ImagingStudiesModel> GetById(ImagingStudiesModel imagingStudiesModel)
    {
        var item = await _imagingStudyRepository.GetById(imagingStudiesModel.ImagingStudyItem.Id);
        if (item is null)
        {
            imagingStudiesModel.IsNotValid = true;
            imagingStudiesModel.Message = "ImagingStudy not found.";
            return imagingStudiesModel;
        }

        imagingStudiesModel.ImagingStudyItem = item;
        imagingStudiesModel.ImagingStudyResponse = ToResponse(item);
        imagingStudiesModel.IsNotValid = false;
        imagingStudiesModel.Message = "ImagingStudy retrieved successfully.";
        return imagingStudiesModel;
    }

    public async Task<ImagingStudiesModel> GetAll(ImagingStudiesModel imagingStudiesModel)
    {
        var items = await _imagingStudyRepository.GetAll();
        imagingStudiesModel.ImagingStudyItems = items;
        imagingStudiesModel.IsNotValid = false;
        imagingStudiesModel.Message = $"{items.Count} imaging study(ies) retrieved.";
        return imagingStudiesModel;
    }

    public async Task<ImagingStudiesModel> GetByPatientId(Guid patientId)
    {
        var items = await _imagingStudyRepository.GetByPatientId(patientId);
        return new ImagingStudiesModel
        {
            ImagingStudyItems = items,
            IsNotValid = false,
            Message = $"{items.Count} imaging study(ies) retrieved for patient."
        };
    }

    public async Task<ImagingStudiesModel> Update(ImagingStudiesModel imagingStudiesModel)
    {
        var existing = await _imagingStudyRepository.GetById(imagingStudiesModel.ImagingStudyItem.Id);
        if (existing is null)
        {
            imagingStudiesModel.IsNotValid = true;
            imagingStudiesModel.Message = "ImagingStudy not found.";
            return imagingStudiesModel;
        }

        var updatedItem = ToItem(imagingStudiesModel.ImagingStudyRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _imagingStudyRepository.Update(updatedItem);
        imagingStudiesModel.ImagingStudyItem = savedItem!;
        imagingStudiesModel.ImagingStudyResponse = ToResponse(savedItem!);
        imagingStudiesModel.IsNotValid = false;
        imagingStudiesModel.Message = "ImagingStudy updated successfully.";
        return imagingStudiesModel;
    }

    public async Task<ImagingStudiesModel> Delete(ImagingStudiesModel imagingStudiesModel)
    {
        var deleted = await _imagingStudyRepository.Delete(imagingStudiesModel.ImagingStudyItem.Id);
        if (!deleted)
        {
            imagingStudiesModel.IsNotValid = true;
            imagingStudiesModel.Message = "ImagingStudy not found.";
            return imagingStudiesModel;
        }

        imagingStudiesModel.IsNotValid = false;
        imagingStudiesModel.Message = "ImagingStudy deleted successfully.";
        return imagingStudiesModel;
    }

    private static ImagingStudyItem ToItem(ImagingStudyRequest request) => new()
    {
        Date = request.Date,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        SeriesUid = request.SeriesUid,
        BodysiteCode = request.BodysiteCode,
        BodysiteDescription = request.BodysiteDescription,
        ModalityCode = request.ModalityCode,
        ModalityDescription = request.ModalityDescription,
        InstanceUid = request.InstanceUid,
        SopCode = request.SopCode,
        SopDescription = request.SopDescription,
        ProcedureCode = request.ProcedureCode
    };

    private static ImagingStudyResponse ToResponse(ImagingStudyItem item) => new()
    {
        Id = item.Id,
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
