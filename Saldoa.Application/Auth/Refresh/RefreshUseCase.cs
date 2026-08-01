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

    public async Task<Result<AuthResult>> ExecuteAsync(string refreshTokenRaw, CancellationToken ct)
    {
        var hash = RefreshTokenCrypto.HashToken(refreshTokenRaw);
        var stored = await _refreshRepo.GetByHashAsync(hash, ct);

        if (stored is null)
        {
            var error = AuthErrors.InvalidAccess;
            return Result<AuthResult>.Failure(error);
        }

        if (stored.IsRevoked)
        {
            if (!string.IsNullOrWhiteSpace(stored.ReplacedByTokenHash))
            {
                await _refreshRepo.RevokeTokenFamilyAsync(stored.ReplacedByTokenHash, ct);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            var error = AuthErrors.InvalidAccess;
            return Result<AuthResult>.Failure(error);
        }

        if (stored.IsExpired)
        {
            var error = AuthErrors.InvalidAccess;
            return Result<AuthResult>.Failure(error);
        }

        var result = await _identityService.GetEmailByUserIdAsync(stored.UserId, ct);
        if (!result.IsSuccess)
        {
            return Result<AuthResult>.Failure(result.Error!);
        }

        var newRefresh = _refreshTokenService.Generate();
        stored.Revoke(replacedByTokenHash: newRefresh.TokenHash);

        var newEntity = new RefreshToken(newRefresh.TokenHash, stored.UserId, newRefresh.ExpiresAt);
        await _refreshRepo.AddAsync(newEntity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var newAccessResult = _jwtProvider.CreateAccessToken(
            userId: stored.UserId,
            email: result.Value!,
            claims: []);

        return Result<AuthResult>.Success(new AuthResult(
            AccessToken: newAccessResult.Token,
            RefreshToken: newRefresh.RawToken,
            AccessTokenExpiresAt: newAccessResult.ExpiresAt,
            RefreshTokenExpiresAt: newRefresh.ExpiresAt
        ));
    }
}
