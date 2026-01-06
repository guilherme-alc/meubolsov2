using MeuBolso.Application.Auth.Abstractions;
using MeuBolso.Application.Common.Results;

namespace MeuBolso.Application.Auth.Logout;

public sealed class LogoutUseCase
{
    private readonly IRefreshTokenRepository _refreshRepo;

    public LogoutUseCase(IRefreshTokenRepository refreshRepo)
        => _refreshRepo = refreshRepo;

    public async Task<Result> ExecuteAsync(string refreshTokenRaw)
    {
        var hash = RefreshTokenCrypto.HashToken(refreshTokenRaw);
        var stored = await _refreshRepo.GetByHashAsync(hash);
        
        if (stored is null)
            return Result.Success();

        if (!stored.IsRevoked)
        {
            stored.Revoke();
            await _refreshRepo.SaveChangesAsync();
        }

        return Result.Success();
    }
}