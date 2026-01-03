using FluentValidation;
using MeuBolso.Application.Auth.AuthDTO;
using MeuBolso.Application.Auth.Register;

namespace MeuBolso.API.Endpoints.Auth;

public static class RegisterEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/register", async (
            RegisterRequest request,
            IValidator<RegisterRequest> validator,
            RegisterUseCase useCase) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var result = await useCase.ExecuteAsync(request);
            
            return !result.IsSuccess ? Results.BadRequest(result.Error) : Results.Created("/auth/register", result.Value);
        });
    }
}