using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.Create;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.Transactions;

public static class CreateTransactionEndpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", 
            async Task<IResult> (
                CreateTransactionRequest request,
                CreateTransactionUseCase useCase,
                IValidator<CreateTransactionRequest> validator,
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
                    var statusCode = MapStatusCode.GetCode(error.Type);

                    return TypedResults.Problem(
                        detail: error.Message,
                        statusCode: statusCode,
                        title: error.Code
                    );
                }

                var response = result.Value!;
                var responseItem = response!.Transactions.FirstOrDefault();

                return TypedResults.Created($"/transactions/{responseItem!.Id}", response);
            }
        )
        .WithSummary("Cria nova transação")
        .WithDescription(
            "Cria nova categoria Título (obrigatório); Descrição (opcionail); Data de Pagamento ou Recebimento (opcionail); " +
            "Tipo da transação (obrigatório): 1 = Despesa, 2 = Renda; Quantia (obrigatório); Id da categoria (obrigatório)"
        );
    }
}