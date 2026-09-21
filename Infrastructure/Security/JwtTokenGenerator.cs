using System.Security.Claims;
using System.Text;
using Application;
using Application.Auth;
using Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public class JwtTokenGenerator(IOptions<AppSettings> appSettings) : IJwtTokenGenerator
{
    private readonly JwtSettings _settings = appSettings.Value.Jwt;

    public JwtToken GenerateToken(Guid userId, string userName, string lastName, string firstName, string roleName, Guid companyId, string companyName, string companyType, IEnumerable<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(CustomClaimTypes.UserName, userName),
            new(CustomClaimTypes.Role, roleName),
            new(CustomClaimTypes.LastName, lastName),
            new(CustomClaimTypes.FirstName, firstName),
            new(CustomClaimTypes.CompanyId, companyId.ToString()),
            new(CustomClaimTypes.CompanyName, companyName),
            new(CustomClaimTypes.CompanyType, companyType),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(permissions.Select(permission => new Claim(CustomClaimTypes.Permission, permission)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Expires = expiresAt,
            SigningCredentials = credentials
        };

        var accessToken = new JsonWebTokenHandler().CreateToken(descriptor);
        return new JwtToken(accessToken, expiresAt);
    }
}
