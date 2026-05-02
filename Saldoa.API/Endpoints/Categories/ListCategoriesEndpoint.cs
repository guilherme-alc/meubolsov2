using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Extensions;
using Saldoa.Application.Categories.List;

namespace Saldoa.API.Endpoints.Categories;

public static class ListCategoriesEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", 
            async Task<IResult> (
                [AsParameters] ListCategoriesRequest request,
                IValidator<ListCategoriesRequest> validator,
                ListCategoriesUseCase useCase,
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
            
                var result = await useCase.ExecuteAsync(userId, request.PageNumber, request.PageSize, ct);

                return TypedResults.Ok(result);
            }
        )
        .WithSummary("Obtém lista de categorias")
        .WithDescription("Obtém lista de categorias do usuário com paginação");
    }
}