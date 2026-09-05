using Saldoa.Application.Auth.Abstractions;
using Saldoa.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Saldoa.Infrastructure.Persistence;

namespace Saldoa.Infrastructure.Auth;

public class RefreshTokenRepository(SaldoaDbContext dbContext) : IRefreshTokenRepository
{
    public Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default)
        => dbContext.RefreshTokens.AddAsync(refreshToken, ct).AsTask();

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default)
        => dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

    public async Task RevokeTokenFamilyAsync(string tokenHash, CancellationToken ct = default)
    {
        var nextTokenHash = tokenHash;
        var visitedTokenHashes = new HashSet<string>(StringComparer.Ordinal);

        while (!string.IsNullOrWhiteSpace(nextTokenHash) &&
               visitedTokenHashes.Add(nextTokenHash))
        {
            var refreshToken = await GetByHashAsync(nextTokenHash, ct);
            if (refreshToken is null)
                return;

            nextTokenHash = refreshToken.ReplacedByTokenHash;

            if (!refreshToken.IsRevoked)
                refreshToken.Revoke();
        }
    }

    public Task CleanExpiredTokensAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        return dbContext.RefreshTokens
            .Where(x => x.ExpiresAt <= now)
            .ExecuteDeleteAsync(ct);
    }
}
