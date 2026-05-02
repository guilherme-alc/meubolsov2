using Saldoa.Application.Auth.Logout;
using Saldoa.Application.Auth.Refresh;

namespace Saldoa.API.Endpoints.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Auth")
            .AllowAnonymous();
        
        RegisterEndpoint.Map(group);
        LoginEndpoint.Map(group);
        LogoutEndpoint.Map(group);
        RefreshEndpoint.Map(group);
    }
}