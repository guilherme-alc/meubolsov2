using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.CategoryBudgets.GetCategoryBudgetById;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.CategoryBudgets;

public static class GetCategoryBudgetByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:long:min(1)}", 
            async Task<IResult> (
                long id,
                GetCategoryBudgetByIdUseCase useCase,
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

                return TypedResults.Ok(result.Value);
            }
        )
        .WithSummary("Obtém um limite de gasto por categoria")
        .WithDescription("Obtém um limite de gasto pelo Id");
    }
}