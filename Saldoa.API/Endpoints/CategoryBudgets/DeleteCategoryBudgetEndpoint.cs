using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.CategoryBudgets.Delete;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.CategoryBudgets;

public static class DeleteCategoryBudgetEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:long:min(1)}", 
            async Task<IResult> (
            long id,
            DeleteCategoryBudgetUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct) =>
            {
                var userId = user.GetUserId();
            
                var result = await useCase.ExecuteAsync(userId, id, ct);

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
        .WithSummary("Remove um limite de gasto por categoria")
        .WithDescription("Remove um limite de gasto pelo Id");
    }
}