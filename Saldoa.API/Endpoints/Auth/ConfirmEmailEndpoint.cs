using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Security;
using Saldoa.Application.Auth.ConfirmEmail;

namespace Saldoa.API.Endpoints.Auth
{
    internal static class ConfirmEmailEndpoint
    {
        internal static void Map(RouteGroupBuilder authGroup)
        {
            authGroup.MapPost("confirm-email", 
                async Task<IResult> (
                    ConfirmEmailRequest request,
                    IValidator<ConfirmEmailRequest> validator,
                    ConfirmEmailUseCase useCase,
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

                    var result = await useCase.ExecuteAsync(request, ct);
                    if (!result.IsSuccess)
                    {
                        var error = result.Error!;
                        int statusCode = StatusCodeMapper.GetCode(error.Type);

                        return TypedResults.Problem(
                            detail: error.Message,
                            statusCode: statusCode,
                            title: error.Code
                        );
                    }

                    return TypedResults.NoContent();
                }
            )
            .RequireRateLimiting(RateLimitPolicies.EmailConfirmation)
            .WithSummary("Confirma e-mail do usuário")
            .WithDescription("Realiza a confirmação do e-mail utilizando o token recebido por e-mail e o id do usuário. ");
        }
    }
}
