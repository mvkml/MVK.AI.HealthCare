namespace AI.HealthCare.Patient.Models.ClaimTransaction;

public class ClaimTransactionsModel : BaseModel
{
    public ClaimTransactionRequest ClaimTransactionRequest { get; set; } = new();
    public ClaimTransactionItem ClaimTransactionItem { get; set; } = new();
    public List<ClaimTransactionItem> ClaimTransactionItems { get; set; } = new();
    public ClaimTransactionResponse ClaimTransactionResponse { get; set; } = new();
}
