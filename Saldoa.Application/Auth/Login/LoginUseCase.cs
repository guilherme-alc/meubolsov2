using Saldoa.Application.Auth.Abstractions;
using Saldoa.Application.Auth.Common;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Identity.Abstractions;
using Saldoa.Domain.Auth;

namespace Saldoa.Application.Auth.Login;

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
    
    public async Task<Result<AuthResponse>> ExecuteAsync(LoginRequest request, CancellationToken ct)
    {
        var userId = await _identityService
            .ValidateCredentialsAndGetUserIdAsync(request.Email, request.Password, ct);
       
        if (userId is null)
        {
            var error = AuthErrors.InvalidAccess;
            return Result<AuthResponse>.Failure(error);
        }

        var accessTokenResult = _jwtProvider.GenerateToken(
            userId: userId,
            email: request.Email,
            claims: []);
        
        var refreshToken = _refreshTokenService.Generate();
        
        var entity = new RefreshToken(refreshToken.TokenHash, userId, refreshToken.ExpiresAt);
        await _refreshRepo.AddAsync(entity, ct);
        await _refreshRepo.SaveChangesAsync(ct);
        
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