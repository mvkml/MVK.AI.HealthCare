namespace HC.AI.Admin.Models;

/// <summary>
/// Outgoing response payload returned to the client for Admin Sign Up.
/// </summary>
public class AdminSignUpResponse : BaseModel
{
    /// <summary>Primary key of the admin.</summary>
    public int AdminId { get; set; }

    /// <summary>Full name of the admin.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Email address used for login.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>UTC timestamp when the admin was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Whether the admin account is active.</summary>
    public bool IsActive { get; set; } = true;
}
