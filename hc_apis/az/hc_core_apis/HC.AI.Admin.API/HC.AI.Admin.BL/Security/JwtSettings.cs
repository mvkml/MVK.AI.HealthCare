namespace HC.AI.Admin.BL.Security;

/// <summary>
/// JWT signing configuration, bound from the "Jwt" section of appsettings.
/// </summary>
public class JwtSettings
{
    /// <summary>Token issuer (e.g. "HC.AI.Admin.Api").</summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>Token audience (e.g. "HC.AI.Admin.Web").</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>Symmetric signing key. Must be at least 32 characters (256 bits) for HMAC-SHA256.</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Access token lifetime, in minutes.</summary>
    public int ExpiryMinutes { get; set; } = 60;
}
