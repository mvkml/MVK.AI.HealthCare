using AI.HealthCare.Patient.Models.Careplan;

namespace AI.HealthCare.Patient.BL;

public class CareplanValidationService : ICareplanValidationService
{
    public CareplansModel Validate(CareplansModel careplansModel)
    {
        var request = careplansModel.CareplanRequest;

        if (request.PatientId == Guid.Empty)
        {
            careplansModel.IsNotValid = true;
            careplansModel.Message = "PatientId is required.";
            return careplansModel;
        }

        if (request.EncounterId == Guid.Empty)
        {
            careplansModel.IsNotValid = true;
            careplansModel.Message = "EncounterId is required.";
            return careplansModel;
        }

        careplansModel.IsNotValid = false;
        careplansModel.Message = "Validation passed.";
        return careplansModel;
    }
}
