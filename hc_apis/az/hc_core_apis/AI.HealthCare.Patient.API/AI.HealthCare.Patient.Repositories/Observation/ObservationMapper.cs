using AI.HealthCare.Patient.Models.Observation;
using EfObservation = AI.HealthCare.Patient.EF.Entities.Observation;

namespace AI.HealthCare.Patient.Repositories;

public class ObservationMapper : IObservationMapper
{
    public ObservationItem ToModel(EfObservation entity) => new()
    {
        Id = entity.Id,
        Date = entity.Date,
        PatientId = entity.PatientId,
        EncounterId = entity.EncounterId,
        Category = entity.Category,
        Code = entity.Code,
        Description = entity.Description,
        Value = entity.Value,
        Units = entity.Units,
        Type = entity.Type
    };

    public EfObservation ToEntity(ObservationItem item) => new()
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
