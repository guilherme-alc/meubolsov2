using MeuBolso.Application.Auth.AuthDTO;
using MeuBolso.Application.Auth.Login;

namespace MeuBolso.API.Endpoints.Auth;

public class LoginEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login", async (
            LoginRequest request,
            LoginUseCase useCase) =>
        {
            
            var result = await useCase.ExecuteAsync(request);
            
            return !result.IsSuccess ? Results.BadRequest(result.Error) : Results.Created("/auth/register", result.Value);
        });
    }
}