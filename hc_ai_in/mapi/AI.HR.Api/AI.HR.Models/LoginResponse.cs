namespace AI.HR.Models;

/// <summary>
/// Outgoing response payload returned to the client for Login.
/// </summary>
public class LoginResponse : BaseModel
{
    /// <summary>Primary key of the logged-in user.</summary>
    public int UserId { get; set; }

    /// <summary>Full name of the user.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Email address used for login.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Company the user belongs to.</summary>
    public string Company { get; set; } = string.Empty;

    /// <summary>RoleId of the user.</summary>
    public int RoleId { get; set; }

    /// <summary>Signed JWT access token, to be sent as "Authorization: Bearer {Token}" on subsequent requests.</summary>
    public string Token { get; set; } = string.Empty;
}
