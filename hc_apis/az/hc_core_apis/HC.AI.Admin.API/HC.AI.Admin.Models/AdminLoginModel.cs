namespace HC.AI.Admin.Models;

/// <summary>
/// Carrier model passed between Controller, Validation Service, and Business Layer for Login.
/// </summary>
public class AdminLoginModel : BaseModel
{
    /// <summary>Incoming login request data, set by the Controller.</summary>
    public AdminLoginRequest AdminLoginRequest { get; set; } = new();

    /// <summary>Outgoing response data built by the Business Layer, returned to the Controller.</summary>
    public AdminLoginResponse AdminLoginResponse { get; set; } = new();
}
