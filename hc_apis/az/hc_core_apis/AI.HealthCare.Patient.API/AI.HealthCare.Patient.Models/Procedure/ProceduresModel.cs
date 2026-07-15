namespace AI.HealthCare.Patient.Models.Procedure;

public class ProceduresModel : BaseModel
{
    public ProcedureRequest ProcedureRequest { get; set; } = new();
    public ProcedureItem ProcedureItem { get; set; } = new();
    public List<ProcedureItem> ProcedureItems { get; set; } = new();
    public ProcedureResponse ProcedureResponse { get; set; } = new();
}
