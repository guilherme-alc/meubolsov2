using MeuBolso.Application.Auth.Abstractions;
using MeuBolso.Application.Auth.Common;
using MeuBolso.Application.Common.Results;
using MeuBolso.Application.Identity.Abstractions;
using MeuBolso.Domain.Auth;

namespace MeuBolso.Application.Auth.Refresh;

public sealed class RefreshUseCase
{
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IJwtProvider _jwtProvider;
    private readonly IIdentityService _identityService;
    private readonly IRefreshTokenService _refreshTokenService;

    public RefreshUseCase(
        IRefreshTokenRepository refreshRepo,
        IJwtProvider jwtProvider,
        IIdentityService identityService,
        IRefreshTokenService refreshTokenService)
    {
        _refreshRepo = refreshRepo;
        _jwtProvider = jwtProvider;
        _identityService = identityService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<AuthResponse>> ExecuteAsync(string refreshTokenRaw)
    {
        var hash = RefreshTokenCrypto.HashToken(refreshTokenRaw);
        var stored = await _refreshRepo.GetByHashAsync(hash);

        if (stored is null || stored.IsExpired || stored.IsRevoked)
            return Result<AuthResponse>.Failure("Acesso inválido");

        var email = await _identityService.GetEmailByUserIdAsync(stored.UserId);
        if (email is null)
            return Result<AuthResponse>.Failure("Acesso inválido");
        
        var newRefresh = _refreshTokenService.Generate();
        stored.Revoke(replacedByTokenHash: newRefresh.TokenHash);

        var newEntity = new RefreshToken(newRefresh.TokenHash, stored.UserId, newRefresh.ExpiresAt);
        await _refreshRepo.AddAsync(newEntity);
        await _refreshRepo.SaveChangesAsync();

        var newAccessResult = _jwtProvider.GenerateToken(
            userId: stored.UserId,
            email: email,
            claims: []);

        return Result<AuthResponse>.Success(new AuthResponse(
            AccessToken: newAccessResult.Token,
            AccessTokenExpiresAt: newAccessResult.ExpiresAt,
            RefreshToken: newRefresh.RawToken,
            RefreshTokenExpiresAt: newRefresh.ExpiresAt
        ));
    }
}