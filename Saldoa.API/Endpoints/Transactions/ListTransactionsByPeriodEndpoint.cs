using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.ListByPeriod;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.Transactions;

public static class ListTransactionsByPeriodEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", 
            async Task<IResult> (
                [AsParameters] ListTransactionsByPeriodRequest request,
                IValidator<ListTransactionsByPeriodRequest> validator,
                ListTransactionsByPeriodUseCase useCase,
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

                return Results.Ok(result.Value);
            }
        )
        .WithSummary("Obtém lista de transações de um período")
        .WithDescription("Obtém lista de transações de um período com paginação e podendo filtrar pelo tipo (despesa ou renda). " + 
                         "O Período é opcional, caso não seja informado será retornado transações em um range da data atual até um ano atrás");
    }
}