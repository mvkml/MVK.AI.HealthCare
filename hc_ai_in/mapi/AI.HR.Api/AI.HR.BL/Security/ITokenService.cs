using AI.HR.Models;

namespace AI.HR.BL.Security;

/// <summary>
/// Generates signed JWT access tokens for authenticated users.
/// </summary>
public interface ITokenService
{
    /// <summary>Generates a signed JWT containing the user's identity claims.</summary>
    string GenerateToken(UserItem userItem);
}
