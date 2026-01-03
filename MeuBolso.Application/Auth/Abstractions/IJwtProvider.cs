using System.Security.Claims;

namespace MeuBolso.Application.Auth.Abstractions;

public interface IJwtProvider
{
    string GenerateToken(string userId, string email, IEnumerable<Claim> claims);
}