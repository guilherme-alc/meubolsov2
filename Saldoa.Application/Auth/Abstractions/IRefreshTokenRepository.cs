using Saldoa.Domain.Auth;

namespace Saldoa.Application.Auth.Abstractions;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct);
    Task RevokeTokenFamilyAsync(string tokenHash, CancellationToken ct);
    Task CleanExpiredTokensAsync(CancellationToken ct);
}
