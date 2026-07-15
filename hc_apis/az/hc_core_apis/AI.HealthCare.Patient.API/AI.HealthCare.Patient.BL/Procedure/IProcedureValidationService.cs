using AI.HealthCare.Patient.Models.Procedure;

namespace AI.HealthCare.Patient.BL;

public interface IProcedureValidationService
{
    ProceduresModel Validate(ProceduresModel proceduresModel);
}
