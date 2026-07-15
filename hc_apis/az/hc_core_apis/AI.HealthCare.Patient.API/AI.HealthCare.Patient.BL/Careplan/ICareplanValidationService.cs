using AI.HealthCare.Patient.Models.Careplan;

namespace AI.HealthCare.Patient.BL;

public interface ICareplanValidationService
{
    CareplansModel Validate(CareplansModel careplansModel);
}
