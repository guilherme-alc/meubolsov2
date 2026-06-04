using System.Security.Claims;

namespace Saldoa.Application.Auth.Abstractions;
public sealed record AccessTokenResult(string Token, DateTime ExpiresAt);
public interface IJwtProvider
{
    AccessTokenResult CreateAccessToken(string userId, string email, IEnumerable<Claim> claims);
}