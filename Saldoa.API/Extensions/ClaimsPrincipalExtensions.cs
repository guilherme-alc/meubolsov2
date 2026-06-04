using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Saldoa.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
               ?? throw new UnauthorizedAccessException("UserId não encontrado no token.");
    }
}
