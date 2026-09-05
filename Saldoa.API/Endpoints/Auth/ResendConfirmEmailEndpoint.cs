using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Security;
using Saldoa.Application.Auth.ResendConfirmEmail;

namespace Saldoa.API.Endpoints.Auth
{
    internal static class ResendConfirmEmailEndpoint
    {
        internal static void Map(RouteGroupBuilder authGroup)
        {
            authGroup.MapPost("resend-confirm-email", 
                async Task<IResult> (
                    ResendConfirmEmailRequest request,
                    IValidator<ResendConfirmEmailRequest> validator,
                    ResendConfirmEmailUseCase useCase,
                    CancellationToken ct) =>
                {
                    var validation = await validator.ValidateAsync(request, ct);
                    if(!validation.IsValid)
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
                    if(!result.IsSuccess)
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
           .WithSummary("Reenvia confirmação de e-mail")
           .WithDescription("Reenvia confirmação de e-mail para o usuário");
        }
    }
}
