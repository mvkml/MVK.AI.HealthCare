using AI.HealthCare.Patient.Models.Observation;
using EfObservation = AI.HealthCare.Patient.EF.Entities.Observation;

namespace AI.HealthCare.Patient.Repositories;

public interface IObservationMapper
{
    ObservationItem ToModel(EfObservation entity);
    EfObservation ToEntity(ObservationItem item);
}
