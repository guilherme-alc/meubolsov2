using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.GetInstallmentsByGroupId;
using System.Security.Claims;

namespace Saldoa.API.Endpoints.Transactions;

public static class GetInstallmentsByGroupIdEndpoint
{
    public static void Map(RouteGroupBuilder transactionsGroup)
    {
        transactionsGroup.MapGet("/installment-groups/{installmentGroupId:guid}", 
            async Task<IResult> (
                Guid installmentGroupId,
                [AsParameters] GetInstallmentsByGroupIdRequest request,
                GetInstallmentsByGroupIdUseCase useCase,
                IValidator<GetInstallmentsByGroupIdRequest> validator,
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

                var result = await useCase.ExecuteAsync(userId, installmentGroupId, request, ct);

                if (!result.IsSuccess)
                {
                    var error = result.Error!;
                    var statusCode = StatusCodeMapper.GetCode(error.Type);

                    return TypedResults.Problem(
                        detail: error.Message,
                        statusCode: statusCode,
                        title: error.Code
                    );
                }

                return TypedResults.Ok(result.Value);
            }
        )
        .WithSummary("Obtém os detalhes de um parcelamento")
        .WithDescription("Retorna o resumo do parcelamento e suas parcelas com paginação.");
    }
}
