namespace AI.HR.Models;

/// <summary>
/// Incoming request payload for user operations (e.g. Sign Up).
/// </summary>
public class UserRequest
{
    /// <summary>Full name of the user.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Email address used for login.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Company the user belongs to.</summary>
    public string Company { get; set; } = string.Empty;

    /// <summary>Plain-text password supplied by the client (hashed before persisting).</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Selected role's RoleId (from GET api/users/roles). Defaults to "Other" if not provided.</summary>
    public int RoleId { get; set; }
}
