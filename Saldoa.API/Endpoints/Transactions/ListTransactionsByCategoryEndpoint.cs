using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.ListByCategory;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.Transactions;

public static class ListTransactionsByCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/by-category", 
            async Task<IResult> (
                [AsParameters] ListTransactionsByCategoryRequest request,
                IValidator<ListTransactionsByCategoryRequest> validator,
                ListTransactionsByCategoryUseCase useCase,
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

                return TypedResults.Ok(result.Value);
            }
        )
        .WithSummary("Obtém lista de transações de uma categoria")
        .WithDescription("Obtém lista de transações de uma categoria específica com paginação e podendo ser filtrada por período");
    }
}