namespace HC.AI.Admin.Models;

/// <summary>
/// Incoming request payload for Admin Sign Up.
/// </summary>
public class AdminSignUpRequest
{
    /// <summary>Full name of the admin.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Email address used for login.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Plain-text password supplied by the client (hashed before persisting).</summary>
    public string Password { get; set; } = string.Empty;
}
