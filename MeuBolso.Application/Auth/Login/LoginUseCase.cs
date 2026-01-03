using MeuBolso.Application.Auth.Abstractions;
using MeuBolso.Application.Auth.AuthDTO;
using MeuBolso.Application.Common.Results;
using MeuBolso.Application.Identity.Abstractions;

namespace MeuBolso.Application.Auth.Login;

public class LoginUseCase
{
    private readonly IIdentityService _identityService;
    private readonly IJwtProvider _jwtProvider;
    
    public LoginUseCase(IIdentityService identityService, IJwtProvider jwt)
    {
        _identityService = identityService;
        _jwtProvider = jwt;
    }
    
    public async Task<Result<AuthResponse>> ExecuteAsync(LoginRequest request)
    {
        var valid = await _identityService
            .ValidateCredentialsAsync(request.Email, request.Password);

       
        if (!valid)
            return Result<AuthResponse>.Failure("Usuário não cadastrado");

        var accessToken = _jwtProvider.GenerateToken(
            userId: request.Email,
            email: request.Email,
            claims: []);

        return Result<AuthResponse>.Success(
            new AuthResponse(accessToken, "", DateTime.UtcNow.AddMinutes(15))
        );
    }
}