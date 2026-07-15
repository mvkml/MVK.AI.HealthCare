using AI.HealthCare.Patient.Models.Procedure;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IProcedureBLMapper : ICsvRowParser<ProcedureItem>
{
    ProcedureItem ToItem(ProcedureRequest request);
    ProcedureResponse ToResponse(ProcedureItem item);
}
