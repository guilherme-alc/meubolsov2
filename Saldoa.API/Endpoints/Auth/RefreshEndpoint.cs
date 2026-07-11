using Saldoa.API.Common;
using Saldoa.Application.Auth.Refresh;

namespace Saldoa.API.Endpoints.Auth;

public static class RefreshEndpoint
{
    public static void Map(RouteGroupBuilder authGroup)
    {
        authGroup.MapPost("/refresh", 
            async Task<IResult> (
                HttpContext httpContext,
                IConfiguration configuration,
                RefreshUseCase useCase,
                CancellationToken ct) =>
            {
                if (!AuthRequestSecurity.IsTrustedRefreshRequest(httpContext.Request, configuration))
                    return AuthRequestSecurity.InvalidOriginProblem();

                var refreshToken = RefreshTokenCookie.Read(httpContext.Request);

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    RefreshTokenCookie.Delete(httpContext.Response);

                    return TypedResults.Problem(
                        title: "Auth.InvalidAccess",
                        detail: "Acesso inválido",
                        statusCode: StatusCodes.Status401Unauthorized
                    );
                }

                var result = await useCase.ExecuteAsync(refreshToken, ct);

                if (!result.IsSuccess)
                {
                    var error = result.Error!;
                    int statusCode = StatusCodeMapper.GetCode(error.Type);

                    RefreshTokenCookie.Delete(httpContext.Response);

                    return TypedResults.Problem(
                        detail: error.Message,
                        statusCode: statusCode,
                        title: error.Code
                    );
                }

                var authResult = result.Value!;
                RefreshTokenCookie.Append(
                    httpContext.Response,
                    authResult.RefreshToken,
                    authResult.RefreshTokenExpiresAt);

                return TypedResults.Ok(authResult.ToResponse());
            }
        )
        .WithSummary("Renova o Access Token")
        .WithDescription(
            "Gera um novo Access Token a partir do Refresh Token armazenado no cookie seguro. " +
            "O Refresh Token utilizado é revogado e substituído por um novo (rotação de token). " +
            "Retorna 401 caso o token esteja inválido, expirado ou revogado."
        );
    }
}
