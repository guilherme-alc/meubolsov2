using Saldoa.Application.Auth.Logout;

namespace Saldoa.API.Endpoints.Auth;

public class LogoutEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/logout", async Task<IResult> (
            HttpRequest http,
            LogoutRequest request,
            LogoutUseCase useCase,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return TypedResults.NoContent(); // NoContent por segurança

            await useCase.ExecuteAsync(request.RefreshToken, ct);
            return TypedResults.NoContent();
        })
        .WithSummary("Encerra a sessão do usuário")
        .WithDescription(
            "Revoga o Refresh Token informado, invalidando futuras renovações de sessão. " +
            "Por segurança, sempre retorna 204 mesmo que o token não exista ou já esteja revogado."
        );
    }
}