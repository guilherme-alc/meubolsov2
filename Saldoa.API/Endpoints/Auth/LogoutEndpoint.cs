using Saldoa.Application.Auth.Logout;

namespace Saldoa.API.Endpoints.Auth;

public class LogoutEndpoint
{
    public static void Map(RouteGroupBuilder authGroup)
    {
        authGroup.MapPost("/logout", 
            async Task<IResult> (
                HttpContext httpContext,
                IConfiguration configuration,
                LogoutUseCase useCase,
                CancellationToken ct) =>
            {
                if (!AuthRequestSecurity.IsTrustedRefreshRequest(httpContext.Request, configuration))
                    return AuthRequestSecurity.InvalidOriginProblem();

                var refreshToken = RefreshTokenCookie.Read(httpContext.Request);

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    RefreshTokenCookie.Delete(httpContext.Response);
                    return TypedResults.NoContent();
                }

                await useCase.ExecuteAsync(refreshToken, ct);
                RefreshTokenCookie.Delete(httpContext.Response);
                return TypedResults.NoContent();
            }
        )
        .WithSummary("Encerra a sessão do usuário")
        .WithDescription(
            "Revoga o Refresh Token armazenado no cookie seguro, invalidando futuras renovações de sessão. " +
            "Por segurança, sempre retorna 204 mesmo que o token não exista ou já esteja revogado."
        );
    }
}
