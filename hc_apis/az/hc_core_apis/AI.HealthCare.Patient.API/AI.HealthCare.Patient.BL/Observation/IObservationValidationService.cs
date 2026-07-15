using AI.HealthCare.Patient.Models.Observation;

namespace AI.HealthCare.Patient.BL;

public interface IObservationValidationService
{
    ObservationsModel Validate(ObservationsModel observationsModel);
}
