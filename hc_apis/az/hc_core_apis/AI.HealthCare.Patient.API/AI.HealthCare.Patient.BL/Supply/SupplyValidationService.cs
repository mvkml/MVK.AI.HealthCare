using AI.HealthCare.Patient.Models.Supply;

namespace AI.HealthCare.Patient.BL;

public class SupplyValidationService : ISupplyValidationService
{
    public SuppliesModel Validate(SuppliesModel suppliesModel)
    {
        var request = suppliesModel.SupplyRequest;

        if (request.PatientId == Guid.Empty)
        {
            suppliesModel.IsNotValid = true;
            suppliesModel.Message = "PatientId is required.";
            return suppliesModel;
        }

        if (request.EncounterId == Guid.Empty)
        {
            suppliesModel.IsNotValid = true;
            suppliesModel.Message = "EncounterId is required.";
            return suppliesModel;
        }

        if (request.Date == default)
        {
            suppliesModel.IsNotValid = true;
            suppliesModel.Message = "Date is required.";
            return suppliesModel;
        }

        suppliesModel.IsNotValid = false;
        suppliesModel.Message = "Validation passed.";
        return suppliesModel;
    }
}
