namespace HC.AI.Admin.Models;

/// <summary>
/// Incoming request payload for Admin Login.
/// </summary>
public class AdminLoginRequest
{
    /// <summary>Email address used for login.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Plain-text password supplied by the client.</summary>
    public string Password { get; set; } = string.Empty;
}
