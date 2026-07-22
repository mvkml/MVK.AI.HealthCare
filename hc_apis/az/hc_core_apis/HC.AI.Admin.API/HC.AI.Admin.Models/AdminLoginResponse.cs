namespace HC.AI.Admin.Models;

/// <summary>
/// Outgoing response payload returned to the client for Admin Login.
/// </summary>
public class AdminLoginResponse : BaseModel
{
    /// <summary>Primary key of the logged-in admin.</summary>
    public int AdminId { get; set; }

    /// <summary>Full name of the admin.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Email address used for login.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Signed JWT access token, to be sent as "Authorization: Bearer {Token}" on subsequent requests.</summary>
    public string Token { get; set; } = string.Empty;
}
