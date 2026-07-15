namespace AI.HealthCare.Patient.Models.Claim;

public class ClaimsModel : BaseModel
{
    public ClaimRequest ClaimRequest { get; set; } = new();
    public ClaimItem ClaimItem { get; set; } = new();
    public List<ClaimItem> ClaimItems { get; set; } = new();
    public ClaimResponse ClaimResponse { get; set; } = new();
}
