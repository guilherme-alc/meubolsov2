using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.Categories.GetById;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.Categories;

public static class GetCategoryByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:long:min(1)}", 
            async Task<IResult> (
                long id,
                GetCategoryByIdUseCase useCase,
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
            
                return TypedResults.Ok(result.Value);
            }
        )
        .WithSummary("Obtém uma categoria")
        .WithDescription("Obtém uma categoria pelo Id");
    }
}