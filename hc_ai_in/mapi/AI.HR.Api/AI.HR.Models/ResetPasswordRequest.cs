namespace AI.HR.Models;

/// <summary>
/// Incoming request payload for Reset Password.
/// </summary>
public class ResetPasswordRequest
{
    /// <summary>Email address of the account to reset.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>New plain-text password supplied by the client.</summary>
    public string NewPassword { get; set; } = string.Empty;
}
