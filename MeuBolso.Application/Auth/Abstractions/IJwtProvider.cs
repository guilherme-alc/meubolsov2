using System.Security.Claims;

namespace MeuBolso.Application.Auth.Abstractions;
public sealed record AccessTokenResult(string Token, DateTime ExpiresAt);
public interface IJwtProvider
{
    AccessTokenResult GenerateToken(string userId, string email, IEnumerable<Claim> claims);
}