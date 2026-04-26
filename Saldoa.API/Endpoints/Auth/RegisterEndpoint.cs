using FluentValidation;
using Saldoa.Application.Auth.Register;

namespace Saldoa.API.Endpoints.Auth;

public static class RegisterEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/register", async (
            RegisterRequest request,
            IValidator<RegisterRequest> validator,
            RegisterUseCase useCase,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var result = await useCase.ExecuteAsync(request, ct);
            
            return !result.IsSuccess ? Results.BadRequest(result.Error) : Results.Created("/auth/register", result.Value);
        })
        .WithSummary("Cadastra usuário")
        .WithDescription(
            "Realiza a autenticação utilizando e-mail e senha. " +
            "Em caso de sucesso, retorna um Access Token (JWT) e um Refresh Token com suas respectivas datas de expiração. " +
            "Em caso de credenciais inválidas, retorna 401."
        );
    }
}