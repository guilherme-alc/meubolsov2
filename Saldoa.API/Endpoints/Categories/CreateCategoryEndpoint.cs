using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.Categories.Create;
using Saldoa.Application.Common.Results;

namespace Saldoa.API.Endpoints.Categories;

public static class CreateCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async Task<IResult>(
            CreateCategoryRequest request,
            CreateCategoryUseCase useCase,
            IValidator<CreateCategoryRequest> validator,
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

            var result = await useCase.ExecuteAsync(request, userId, ct);
            
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
            
            var response = result.Value!;
            
            return TypedResults.Created($"/categories/{response.Id}", response);
        })
        .WithSummary("Cria nova categoria")
        .WithDescription("Cria nova categoria Nome (obrigatório), descrição e cor (opcionais)");
    }
}