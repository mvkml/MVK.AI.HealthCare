using AI.HealthCare.Patient.Models.Observation;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ObservationBL : IObservationBL
{
    private readonly IObservationRepository _observationRepository;

    public ObservationBL(IObservationRepository observationRepository)
    {
        _observationRepository = observationRepository;
    }

    public async Task<ObservationsModel> Create(ObservationsModel observationsModel)
    {
        observationsModel.ObservationItem = ToItem(observationsModel.ObservationRequest);

        var savedItem = await _observationRepository.Create(observationsModel.ObservationItem);
        observationsModel.ObservationItem = savedItem;

        observationsModel.ObservationResponse = ToResponse(savedItem);
        observationsModel.IsNotValid = false;
        observationsModel.Message = "Observation created successfully.";

        return observationsModel;
    }

    public async Task<ObservationsModel> GetById(ObservationsModel observationsModel)
    {
        var item = await _observationRepository.GetById(observationsModel.ObservationItem.Id);
        if (item is null)
        {
            observationsModel.IsNotValid = true;
            observationsModel.Message = "Observation not found.";
            return observationsModel;
        }

        observationsModel.ObservationItem = item;
        observationsModel.ObservationResponse = ToResponse(item);
        observationsModel.IsNotValid = false;
        observationsModel.Message = "Observation retrieved successfully.";
        return observationsModel;
    }

    public async Task<ObservationsModel> GetAll(ObservationsModel observationsModel)
    {
        var items = await _observationRepository.GetAll();
        observationsModel.ObservationItems = items;
        observationsModel.IsNotValid = false;
        observationsModel.Message = $"{items.Count} observation(s) retrieved.";
        return observationsModel;
    }

    public async Task<ObservationsModel> GetByPatientId(Guid patientId)
    {
        var items = await _observationRepository.GetByPatientId(patientId);
        return new ObservationsModel
        {
            ObservationItems = items,
            IsNotValid = false,
            Message = $"{items.Count} observation(s) retrieved for patient."
        };
    }

    public async Task<ObservationsModel> Update(ObservationsModel observationsModel)
    {
        var existing = await _observationRepository.GetById(observationsModel.ObservationItem.Id);
        if (existing is null)
        {
            observationsModel.IsNotValid = true;
            observationsModel.Message = "Observation not found.";
            return observationsModel;
        }

        var updatedItem = ToItem(observationsModel.ObservationRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _observationRepository.Update(updatedItem);
        observationsModel.ObservationItem = savedItem!;
        observationsModel.ObservationResponse = ToResponse(savedItem!);
        observationsModel.IsNotValid = false;
        observationsModel.Message = "Observation updated successfully.";
        return observationsModel;
    }

    public async Task<ObservationsModel> Delete(ObservationsModel observationsModel)
    {
        var deleted = await _observationRepository.Delete(observationsModel.ObservationItem.Id);
        if (!deleted)
        {
            observationsModel.IsNotValid = true;
            observationsModel.Message = "Observation not found.";
            return observationsModel;
        }

        observationsModel.IsNotValid = false;
        observationsModel.Message = "Observation deleted successfully.";
        return observationsModel;
    }

    private static ObservationItem ToItem(ObservationRequest request) => new()
    {
        Date = request.Date,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        Category = request.Category,
        Code = request.Code,
        Description = request.Description,
        Value = request.Value,
        Units = request.Units,
        Type = request.Type
    };

    private static ObservationResponse ToResponse(ObservationItem item) => new()
    {
        Id = item.Id,
        Date = item.Date,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Category = item.Category,
        Code = item.Code,
        Description = item.Description,
        Value = item.Value,
        Units = item.Units,
        Type = item.Type
    };
}
