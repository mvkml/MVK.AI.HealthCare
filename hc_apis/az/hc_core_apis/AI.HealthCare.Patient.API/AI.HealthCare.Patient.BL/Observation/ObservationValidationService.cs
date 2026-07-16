using AI.HealthCare.Patient.Models.Observation;

namespace AI.HealthCare.Patient.BL;

public class ObservationValidationService : IObservationValidationService
{
    public ObservationsModel Validate(ObservationsModel observationsModel)
    {
        var request = observationsModel.ObservationRequest;

        if (request.PatientId == Guid.Empty)
        {
            observationsModel.IsNotValid = true;
            observationsModel.Message = "PatientId is required.";
            return observationsModel;
        }

        if (request.Date == default)
        {
            observationsModel.IsNotValid = true;
            observationsModel.Message = "Date is required.";
            return observationsModel;
        }

        observationsModel.IsNotValid = false;
        observationsModel.Message = "Validation passed.";
        return observationsModel;
    }
}
