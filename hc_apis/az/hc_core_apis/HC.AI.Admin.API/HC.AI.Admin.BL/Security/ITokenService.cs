using HC.AI.Admin.Models;

namespace HC.AI.Admin.BL.Security;

/// <summary>
/// Generates signed JWT access tokens for authenticated admins.
/// </summary>
public interface ITokenService
{
    /// <summary>Generates a signed JWT containing the admin's identity claims.</summary>
    string GenerateToken(AdminItem adminItem);
}
