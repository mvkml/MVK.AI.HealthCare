using AI.HealthCare.Patient.Models.Supply;
using EfSupply = AI.HealthCare.Patient.EF.Entities.Supply;

namespace AI.HealthCare.Patient.Repositories;

public interface ISupplyMapper
{
    SupplyItem ToModel(EfSupply entity);
    EfSupply ToEntity(SupplyItem item);
}
