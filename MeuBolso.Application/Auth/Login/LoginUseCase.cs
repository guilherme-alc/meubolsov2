using MeuBolso.Application.Auth.Abstractions;
using MeuBolso.Application.Auth.Common;
using MeuBolso.Application.Common.Results;
using MeuBolso.Application.Identity.Abstractions;
using MeuBolso.Domain.Auth;

namespace MeuBolso.Application.Auth.Login;

public class LoginUseCase
{
    private readonly IIdentityService _identityService;
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IRefreshTokenService _refreshTokenService;
    
    public LoginUseCase(
        IIdentityService identityService, 
        IJwtProvider jwt, 
        IRefreshTokenRepository refreshRepo,
        IRefreshTokenService refreshTokenService)
    {
        _identityService = identityService;
        _jwtProvider = jwt;
        _refreshRepo = refreshRepo;
        _refreshTokenService = refreshTokenService;
    }
    
    public async Task<Result<AuthResponse>> ExecuteAsync(LoginRequest request)
    {
        var userId = await _identityService
            .ValidateCredentialsAndGetUserIdAsync(request.Email, request.Password);
       
        if (userId is null)
            return Result<AuthResponse>.Failure("Acesso inválido");

        var accessTokenResult = _jwtProvider.GenerateToken(
            userId: userId,
            email: request.Email,
            claims: []);
        
        var refreshToken = _refreshTokenService.Generate();
        
        var entity = new RefreshToken(refreshToken.TokenHash, userId, refreshToken.ExpiresAt);
        await _refreshRepo.AddAsync(entity);
        await _refreshRepo.SaveChangesAsync();
        
        return Result<AuthResponse>.Success(
            new AuthResponse(
                AccessToken: accessTokenResult.Token,
                AccessTokenExpiresAt: accessTokenResult.ExpiresAt,
                RefreshToken: refreshToken.RawToken,
                RefreshTokenExpiresAt: refreshToken.ExpiresAt
            )
        );
    }
}