using FluentValidation;
using MeuBolso.Application.Auth.Login;

namespace MeuBolso.API.Endpoints.Auth;

public class LoginEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login", async (
            LoginRequest request,
            IValidator<LoginRequest> validator,
            LoginUseCase useCase) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var result = await useCase.ExecuteAsync(request);
            
            if (!result.IsSuccess)
                return Results.Json(new { message = "Acesso inválido" }, statusCode: 401);
            
            return Results.Ok(result.Value);
        });
    }
}