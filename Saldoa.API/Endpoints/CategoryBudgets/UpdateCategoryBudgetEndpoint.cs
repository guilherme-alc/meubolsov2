using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.CategoryBudgets.Update;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.CategoryBudgets;

public static class UpdateCategoryBudgetEndpoint
{
    public static void Map(RouteGroupBuilder categoryBudgetsGroup)
    {
        categoryBudgetsGroup.MapPut("/{id:long:min(1)}", 
            async Task<IResult> (
                long id,
                UpdateCategoryBudgetRequest request,
                UpdateCategoryBudgetUseCase useCase,
                IValidator<UpdateCategoryBudgetRequest> validator,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                    return TypedResults.ValidationProblem(
                        errors: errors,
                        detail: "Um ou mais campos possuem erros de validação.",
                        title: "Entrada inválida"
                    );
                }

                var userId = user.GetUserId();
            
                var result = await useCase.ExecuteAsync(userId, id, request, ct);

                if (!result.IsSuccess)
                {
                    var error = result.Error!;
                    var statusCode = StatusCodeMapper.GetCode(error.Type);

                    return TypedResults.Problem(
                        detail: error.Message,
                        statusCode: statusCode,
                        title: error.Code
                    );
                }

                return TypedResults.NoContent();
            }
        )
        .WithSummary("Atualiza limite de gasto da categoria")
        .WithDescription(
            "Atualiza completamente o limite de gasto. " +
            "Não será possível atualizar limites já expirados, ou que a atualização crie sobreposição de limites já existentes"
        );
    }
}