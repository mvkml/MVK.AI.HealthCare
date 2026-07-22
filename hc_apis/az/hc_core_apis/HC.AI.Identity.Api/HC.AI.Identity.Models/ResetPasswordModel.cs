namespace HC.AI.Identity.Models;

/// <summary>
/// Carrier model passed between Controller, Validation Service, and Business Layer for Reset Password.
/// </summary>
public class ResetPasswordModel : BaseModel
{
    /// <summary>Incoming reset-password request data, set by the Controller.</summary>
    public ResetPasswordRequest ResetPasswordRequest { get; set; } = new();

    /// <summary>Outgoing response data built by the Business Layer, returned to the Controller.</summary>
    public ResetPasswordResponse ResetPasswordResponse { get; set; } = new();
}
