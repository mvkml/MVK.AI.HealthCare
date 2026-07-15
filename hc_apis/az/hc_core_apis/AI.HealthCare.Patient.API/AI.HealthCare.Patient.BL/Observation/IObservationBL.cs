using AI.HealthCare.Patient.Models.Observation;

namespace AI.HealthCare.Patient.BL;

public interface IObservationBL
{
    Task<ObservationsModel> Create(ObservationsModel observationsModel);
    Task<ObservationsModel> GetById(ObservationsModel observationsModel);
    Task<ObservationsModel> GetAll(ObservationsModel observationsModel);
    Task<ObservationsModel> GetByPatientId(Guid patientId);
    Task<ObservationsModel> Update(ObservationsModel observationsModel);
    Task<ObservationsModel> Delete(ObservationsModel observationsModel);
}
