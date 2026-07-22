namespace HC.AI.Identity.Models;

/// <summary>
/// Incoming request payload for Forgot Password.
/// </summary>
public class ForgotPasswordRequest
{
    /// <summary>Email address of the account to reset.</summary>
    public string Email { get; set; } = string.Empty;
}
