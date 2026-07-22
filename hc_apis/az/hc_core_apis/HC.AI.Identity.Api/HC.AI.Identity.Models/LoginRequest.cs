namespace HC.AI.Identity.Models;

/// <summary>
/// Incoming request payload for Login.
/// </summary>
public class LoginRequest
{
    /// <summary>Email address used for login.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Plain-text password supplied by the client.</summary>
    public string Password { get; set; } = string.Empty;
}
