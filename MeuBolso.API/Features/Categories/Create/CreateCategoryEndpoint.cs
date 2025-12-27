using System.Security.Claims;
using FluentValidation;
using MeuBolso.API.Extensions;
using MeuBolso.Application.Categories.Create;

namespace MeuBolso.API.Features.Categories.Create;

public static class CreateCategoryEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/categories", async (
            CreateCategoryRequest request,
            CreateCategoryUseCase useCase,
            IValidator<CreateCategoryRequest> validator,
            ClaimsPrincipal user) =>
        {
            var userId = user.GetUserId();

            request.UserId = userId;

            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);

            var response = await useCase.ExecuteAsync(request);

            return Results.Created($"/categories/{response.id}", new { result = response });
        });
    }
}