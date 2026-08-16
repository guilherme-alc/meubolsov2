using FluentValidation;
using Saldoa.API.Common;
using Saldoa.API.Security;
using Saldoa.Application.Auth.PasswordReset;
using Saldoa.Application.Auth.ResetPassword;

namespace Saldoa.API.Endpoints.Auth
{
    public static class ResetPasswordEndpoint
    {
        public static void Map(RouteGroupBuilder authGroup)
        {
            authGroup.MapPost("/reset-password",
                async Task<IResult> (
                    ResetPasswordRequest request,
                    IValidator<ResetPasswordRequest> validator,
                    ResetPasswordUseCase useCase,
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
            .RequireRateLimiting(RateLimitPolicies.PasswordResetConfirm)
            .WithSummary("Redefinir senha do usuário")
            .WithDescription("Realiza a redefinição da senha utilizando o token recebido por e-mail, o id do usuário e a nova senha.");
        }
    }
}
