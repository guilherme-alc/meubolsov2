namespace MeuBolso.API.Endpoints.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .AllowAnonymous();
        
        RegisterEndpoint.Map(group);
        LoginEndpoint.Map(group);
    }
}