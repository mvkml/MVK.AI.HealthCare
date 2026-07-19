namespace AI.HR.Models;

/// <summary>
/// Carrier model passed between Controller, Validation Service, and Business Layer for Forgot Password.
/// </summary>
public class ForgotPasswordModel : BaseModel
{
    /// <summary>Incoming forgot-password request data, set by the Controller.</summary>
    public ForgotPasswordRequest ForgotPasswordRequest { get; set; } = new();

    /// <summary>Outgoing response data built by the Business Layer, returned to the Controller.</summary>
    public ForgotPasswordResponse ForgotPasswordResponse { get; set; } = new();
}
