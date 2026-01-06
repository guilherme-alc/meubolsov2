using MeuBolso.Application.Auth.Logout;
using MeuBolso.Application.Auth.Refresh;

namespace MeuBolso.API.Endpoints.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .AllowAnonymous();
        
        RegisterEndpoint.Map(group);
        LoginEndpoint.Map(group);
        
        
        group.MapPost("/refresh", async (
            RefreshRequest request,
            RefreshUseCase useCase) =>
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Results.Unauthorized();

            var result = await useCase.ExecuteAsync(request.RefreshToken);
            if (!result.IsSuccess)
                return Results.Unauthorized();

            return Results.Ok(result.Value);
        });

        group.MapPost("/logout", async (
            HttpRequest http,
            LogoutRequest request,
            LogoutUseCase useCase) =>
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Results.NoContent(); // NoContent por segurança

            await useCase.ExecuteAsync(request.RefreshToken);
            return Results.NoContent();
        });
    }
}