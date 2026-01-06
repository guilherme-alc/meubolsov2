using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MeuBolso.Application.Auth.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MeuBolso.API.Auth;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }
    
    public AccessTokenResult GenerateToken(string userId, string email, IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        
        var jwtClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        jwtClaims.AddRange(claims);

        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);
        
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: jwtClaims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        
        return new AccessTokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt
        );
    }
}