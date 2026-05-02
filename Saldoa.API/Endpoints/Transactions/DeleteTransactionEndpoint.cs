using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.Delete;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.Transactions;

public static class DeleteTransactionEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:long}", 
            async Task<IResult> (
                long id,
                DeleteTransactionUseCase useCase,
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

                return TypedResults.NoContent();
            }
        )
        .WithSummary("Remove uma transação")
        .WithDescription("Remove transação pelo Id");
    }
}