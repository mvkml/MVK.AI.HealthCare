using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AI.HR.Models;
using Microsoft.IdentityModel.Tokens;

namespace AI.HR.BL.Security;

/// <summary>
/// Generates signed JWT access tokens for authenticated users.
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    /// <summary>Creates the service with the given JWT signing settings.</summary>
    public TokenService(JwtSettings settings)
    {
        _settings = settings;
    }

    /// <summary>Generates a signed JWT containing the user's identity claims.</summary>
    public string GenerateToken(UserItem userItem)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userItem.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, userItem.Email),
            new Claim(ClaimTypes.Role, userItem.RoleId.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
