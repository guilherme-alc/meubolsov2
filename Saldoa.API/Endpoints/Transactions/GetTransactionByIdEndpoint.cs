using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.GetById;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.Transactions;

public static class GetTransactionByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:long}", 
            async Task<IResult> (
                long id,
                GetTransactionByIdUseCase useCase,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var userId = user.GetUserId();

                var result = await useCase.ExecuteAsync(id, userId, ct);

                if (!result.IsSuccess)
                {
                    var error = result.Error!;
                    var statusCode = MapStatusCode.GetCode(error.Type);

                    return TypedResults.Problem(
                        detail: error.Message,
                        statusCode: statusCode,
                        title: error.Code
                    );
                }

                var response = result.Value!;

                return TypedResults.Ok(response);
            }
        )
        .WithSummary("Obtém uma transação")
        .WithDescription("Obtém uma transação pelo Id");
    }
}