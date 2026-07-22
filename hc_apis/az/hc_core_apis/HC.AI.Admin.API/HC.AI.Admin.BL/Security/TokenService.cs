using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HC.AI.Admin.Models;
using Microsoft.IdentityModel.Tokens;

namespace HC.AI.Admin.BL.Security;

/// <summary>
/// Generates signed JWT access tokens for authenticated admins.
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    /// <summary>Creates the service with the given JWT signing settings.</summary>
    public TokenService(JwtSettings settings)
    {
        _settings = settings;
    }

    /// <summary>Generates a signed JWT containing the admin's identity claims.</summary>
    public string GenerateToken(AdminItem adminItem)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, adminItem.AdminId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, adminItem.Email),
            new Claim(ClaimTypes.Role, "Admin"),
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
