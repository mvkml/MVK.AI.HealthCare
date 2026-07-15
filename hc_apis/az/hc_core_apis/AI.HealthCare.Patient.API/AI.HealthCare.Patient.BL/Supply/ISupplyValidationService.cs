using AI.HealthCare.Patient.Models.Supply;

namespace AI.HealthCare.Patient.BL;

public interface ISupplyValidationService
{
    SuppliesModel Validate(SuppliesModel suppliesModel);
}
