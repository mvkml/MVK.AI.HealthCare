using AI.HealthCare.Patient.Models.Procedure;

namespace AI.HealthCare.Patient.BL;

public class ProcedureValidationService : IProcedureValidationService
{
    public ProceduresModel Validate(ProceduresModel proceduresModel)
    {
        var request = proceduresModel.ProcedureRequest;

        if (request.PatientId == Guid.Empty)
        {
            proceduresModel.IsNotValid = true;
            proceduresModel.Message = "PatientId is required.";
            return proceduresModel;
        }

        if (request.EncounterId == Guid.Empty)
        {
            proceduresModel.IsNotValid = true;
            proceduresModel.Message = "EncounterId is required.";
            return proceduresModel;
        }

        proceduresModel.IsNotValid = false;
        proceduresModel.Message = "Validation passed.";
        return proceduresModel;
    }
}
