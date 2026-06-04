using Saldoa.Application.Auth.Abstractions;
using Saldoa.Application.Auth.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Identity.Abstractions;
using Saldoa.Domain.Auth;

namespace Saldoa.Application.Auth.Refresh;

public sealed class RefreshUseCase
{
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IJwtProvider _jwtProvider;
    private readonly IIdentityService _identityService;
    private readonly IRefreshTokenGenerator _refreshTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshUseCase(
        IRefreshTokenRepository refreshRepo,
        IJwtProvider jwtProvider,
        IIdentityService identityService,
        IRefreshTokenGenerator refreshTokenService,
        IUnitOfWork unitOfWork)
    {
        _refreshRepo = refreshRepo;
        _jwtProvider = jwtProvider;
        _identityService = identityService;
        _refreshTokenService = refreshTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> ExecuteAsync(string refreshTokenRaw, CancellationToken ct)
    {
        var hash = RefreshTokenCrypto.HashToken(refreshTokenRaw);
        var stored = await _refreshRepo.GetByHashAsync(hash, ct);

        if (stored is null || stored.IsExpired || stored.IsRevoked)
        {
            var error = AuthErrors.InvalidAccess;
            return Result<AuthResponse>.Failure(error);
        }

        var email = await _identityService.GetEmailByUserIdAsync(stored.UserId, ct);
        if (email is null)
        {
            var error = AuthErrors.InvalidAccess;
            return Result<AuthResponse>.Failure(error);
        }

        var newRefresh = _refreshTokenService.Generate();
        stored.Revoke(replacedByTokenHash: newRefresh.TokenHash);

        var newEntity = new RefreshToken(newRefresh.TokenHash, stored.UserId, newRefresh.ExpiresAt);
        await _refreshRepo.AddAsync(newEntity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var newAccessResult = _jwtProvider.CreateAccessToken(
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
