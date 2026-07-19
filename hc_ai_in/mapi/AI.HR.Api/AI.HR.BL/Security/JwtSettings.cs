namespace AI.HR.BL.Security;

/// <summary>
/// JWT signing configuration, bound from the "Jwt" section of appsettings.
/// </summary>
public class JwtSettings
{
    /// <summary>Token issuer (e.g. "AI.HR.Api").</summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>Token audience (e.g. "AI.HR.Web").</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>Symmetric signing key. Must be at least 32 characters (256 bits) for HMAC-SHA256.</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Access token lifetime, in minutes.</summary>
    public int ExpiryMinutes { get; set; } = 60;
}
