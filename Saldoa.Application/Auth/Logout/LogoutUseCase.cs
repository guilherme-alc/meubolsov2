using Saldoa.Application.Auth.Abstractions;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Auth.Logout;

public sealed class LogoutUseCase
{
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutUseCase(IRefreshTokenRepository refreshRepo, IUnitOfWork unitOfWork)
    {
        _refreshRepo = refreshRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(string refreshTokenRaw, CancellationToken ct)
    {
        var hash = RefreshTokenCrypto.HashToken(refreshTokenRaw);
        var stored = await _refreshRepo.GetByHashAsync(hash, ct);
        
        if (stored is null)
            return Result.Success();

        if (!stored.IsRevoked)
        {
            stored.Revoke();
            await _unitOfWork.SaveChangesAsync(ct);
        }

        return Result.Success();
    }
}
