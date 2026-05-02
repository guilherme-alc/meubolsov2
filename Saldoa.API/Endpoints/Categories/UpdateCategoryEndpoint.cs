using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.Categories.Update;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.Categories;

public static class UpdateCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:long}", async Task<IResult> (
            long id,
            UpdateCategoryRequest request,
            UpdateCategoryUseCase useCase,
            IValidator<UpdateCategoryRequest> validator,
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

            var result = await useCase.ExecuteAsync(id, request, userId, ct);
            
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
            
            return TypedResults.NoContent();
        })
        .WithSummary("Atualiza categoria")
        .WithDescription(
            "Atualiza parcialmente a categoria. Apenas propriedades enviadas com valor diferente de null serão alteradas. " +
            "Para limpar um campo opcional, envie uma string vazia."
        );
    }
}