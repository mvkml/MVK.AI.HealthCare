using AI.HealthCare.Patient.Models.Observation;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IObservationBLMapper : ICsvRowParser<ObservationItem>
{
    ObservationItem ToItem(ObservationRequest request);
    ObservationResponse ToResponse(ObservationItem item);
}
