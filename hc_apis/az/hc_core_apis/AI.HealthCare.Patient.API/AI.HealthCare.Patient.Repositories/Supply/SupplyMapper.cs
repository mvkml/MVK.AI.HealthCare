using AI.HealthCare.Patient.Models.Supply;
using EfSupply = AI.HealthCare.Patient.EF.Entities.Supply;

namespace AI.HealthCare.Patient.Repositories;

public class SupplyMapper : ISupplyMapper
{
    public SupplyItem ToModel(EfSupply entity) => new()
    {
        Id = entity.Id,
        Date = entity.Date,
        PatientId = entity.PatientId,
        EncounterId = entity.EncounterId,
        Code = entity.Code,
        Description = entity.Description,
        Quantity = entity.Quantity
    };

    public EfSupply ToEntity(SupplyItem item) => new()
    {
        Id = item.Id,
        Date = item.Date,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        Description = item.Description,
        Quantity = item.Quantity
    };
}
