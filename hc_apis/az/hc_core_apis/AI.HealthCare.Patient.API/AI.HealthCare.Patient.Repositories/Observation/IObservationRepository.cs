using AI.HealthCare.Patient.Models.Observation;

namespace AI.HealthCare.Patient.Repositories;

public interface IObservationRepository
{
    Task<ObservationItem?> GetById(long id);
    Task<List<ObservationItem>> GetAll();
    Task<List<ObservationItem>> GetByPatientId(Guid patientId);
    Task<ObservationItem> Create(ObservationItem observationItem);
    Task<ObservationItem?> Update(ObservationItem observationItem);
    Task<bool> Delete(long id);
}
