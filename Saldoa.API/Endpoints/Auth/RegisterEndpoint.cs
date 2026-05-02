using FluentValidation;
using Saldoa.API.Common;
using Saldoa.Application.Auth.Register;

namespace Saldoa.API.Endpoints.Auth;

public static class RegisterEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/register", 
            async Task<IResult> (
                RegisterRequest request,
                IValidator<RegisterRequest> validator,
                RegisterUseCase useCase,
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
                    int statusCode = MapStatusCode.GetCode(error.Type);

                    return TypedResults.Problem(
                        detail: error.Message,
                        statusCode: statusCode,
                        title: error.Code
                    );
                }

                return TypedResults.NoContent();
            }
        )
        .WithSummary("Cadastra usuário")
        .WithDescription(
            "Realiza a autenticação utilizando e-mail e senha. " +
            "Em caso de sucesso, retorna um Access Token (JWT) e um Refresh Token com suas respectivas datas de expiração. " +
            "Em caso de credenciais inválidas, retorna 401."
        );
    }
}