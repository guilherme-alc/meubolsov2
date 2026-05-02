using Saldoa.API.Common;
using Saldoa.Application.Auth.Refresh;

namespace Saldoa.API.Endpoints.Auth;

public static class RefreshEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/refresh", async Task<IResult> (
            RefreshRequest request,
            RefreshUseCase useCase,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return TypedResults.Problem(
                    title: "Auth.InvalidAccess",
                    detail: "Acesso inválido",
                    statusCode: StatusCodes.Status401Unauthorized
                );

            var result = await useCase.ExecuteAsync(request.RefreshToken, ct);

            if (!result.IsSuccess)
            {
                var error = result.Error!;
                int statusCode = MapStatusCode.GetCode(error.Type);

                return TypedResults.Problem(
                    detail: error.Message,
                    statusCode: statusCode,
                    title: error.Code
                );
            }

            return TypedResults.Ok(result.Value);
        })
        .WithSummary("Renova o Access Token")
        .WithDescription(
            "Gera um novo Access Token a partir de um Refresh Token válido. " +
            "O Refresh Token utilizado é revogado e substituído por um novo (rotação de token). " +
            "Retorna 401 caso o token esteja inválido, expirado ou revogado."
        );
    }
}