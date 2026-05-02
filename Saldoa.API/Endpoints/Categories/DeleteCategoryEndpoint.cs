using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.Categories.Delete;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.Categories;

public static class DeleteCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:long:min(1)}", async (
            long id,
            DeleteCategoryUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();
            
            var result = await useCase.ExecuteAsync(id, userId, ct);
            
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
            
            return Results.NoContent();
        })
        .WithSummary("Remove uma categoria")
        .WithDescription("Remove categoria pelo Id");
    }
}