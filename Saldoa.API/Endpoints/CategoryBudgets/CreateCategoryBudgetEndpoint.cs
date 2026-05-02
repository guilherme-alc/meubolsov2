using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.CategoryBudgets.Create;

namespace Saldoa.API.Endpoints.CategoryBudgets;

public static class CreateCategoryBudgetEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", 
            async Task<IResult> (
                CreateCategoryBudgetRequest request,
                IValidator<CreateCategoryBudgetRequest> validator,
                CreateCategoryBudgetUseCase useCase,
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
            
                var result = await useCase.ExecuteAsync(userId, request, ct);

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
            
                return TypedResults.Created($"/category-budgets/{response.Id}", response);
            }
        )        
        .WithSummary("Cria um limite de gasto por categoria")
        .WithDescription("Cria limite de gasto informando: Id da categoria em que a regra será aplicada, périodo em que essa regra será válida e o limite definido");
    }
}