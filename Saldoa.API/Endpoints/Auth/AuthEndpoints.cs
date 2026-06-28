namespace Saldoa.API.Endpoints.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authGroup = endpoints.MapGroup("/auth")
            .WithTags("Auth")
            .AllowAnonymous();
        
        RegisterEndpoint.Map(authGroup);
        LoginEndpoint.Map(authGroup);
        LogoutEndpoint.Map(authGroup);
        RefreshEndpoint.Map(authGroup);
    }
}