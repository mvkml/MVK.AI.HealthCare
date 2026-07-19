namespace AI.HR.Models;

/// <summary>
/// Carrier model passed between Controller, Validation Service, and Business Layer for Login.
/// </summary>
public class LoginModel : BaseModel
{
    /// <summary>Incoming login request data, set by the Controller.</summary>
    public LoginRequest LoginRequest { get; set; } = new();

    /// <summary>Outgoing response data built by the Business Layer, returned to the Controller.</summary>
    public LoginResponse LoginResponse { get; set; } = new();
}
