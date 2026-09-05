using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Security;
using Saldoa.Application.Auth.SendPasswordReset;

namespace Saldoa.API.Endpoints.Auth
{
    internal static class SendPasswordResetTokenEndpoint
    {
        internal static void Map(RouteGroupBuilder authGroup)
        {
            authGroup.MapPost("/send-password-reset-token", 
                async Task<IResult> (
                    SendPasswordResetTokenRequest request, 
                    IValidator<SendPasswordResetTokenRequest> validator,
                    SendPasswordResetTokenUseCase useCase, 
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
            .RequireRateLimiting(RateLimitPolicies.PasswordResetRequest)
            .WithSummary("Envia e-mail para redefinição de senha")
            .WithDescription("Envia um e-mail para o usuário com um token para redefinir a senha. O e-mail será enviado apenas se o usuário existir e não tiver redefinido a senha recentemente.");
        }
    }
}
