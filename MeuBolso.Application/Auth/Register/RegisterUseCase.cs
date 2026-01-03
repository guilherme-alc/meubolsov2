using MeuBolso.Application.Auth.Abstractions;
using MeuBolso.Application.Auth.AuthDTO;
using MeuBolso.Application.Common.Results;
using MeuBolso.Application.Identity.Abstractions;

namespace MeuBolso.Application.Auth.Register;

public class RegisterUseCase
{
    private readonly IIdentityService _identityService;
    private readonly IJwtProvider _jwtProvider;
    public RegisterUseCase(IIdentityService identityService, IJwtProvider jwtProvider)
    {
        _identityService = identityService;
        _jwtProvider = jwtProvider;
    }
    public async Task<Result<AuthResponse>> ExecuteAsync(RegisterRequest request)
    {
        if (await _identityService.UserExistsAsync(request.Email))
            return Result<AuthResponse>.Failure("Usuário já existe");
        
        var userId = await _identityService.CreateUserAsync(
            request.Email, 
            request.Password, 
            request.FullName);
        
        var accessToken = _jwtProvider.GenerateToken(
            userId: userId,
            email: request.Email,
            claims: []);

        return Result<AuthResponse>.Success(
            new AuthResponse(accessToken, "", DateTime.UtcNow.AddMinutes(15))
        );
    }
}