using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.CategoryBudgets.GetCategoryBudgetByCategory;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.CategoryBudgets;

public static class GetCategoryBudgetsByCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/by-category/{categoryId:long:min(1)}", 
            async Task<IResult> (
            long categoryId,
            [AsParameters] GetCategoryBudgetsByCategoryRequest request,
            IValidator<GetCategoryBudgetsByCategoryRequest> validator,
            GetCategoryBudgetsByCategoryUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if(!validation.IsValid)
                    return Results.BadRequest(validation.Errors);

                var userId = user.GetUserId();

                var result = await useCase.ExecuteAsync(userId, categoryId, request, ct);

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
        .WithSummary("Obtém lista de limite de gasto de uma categoria")
        .WithDescription("Obtém lista de limites de gastos da categoria especificada na rota");
    }
}