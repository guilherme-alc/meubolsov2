using Saldoa.Application.Auth.Abstractions;
using Saldoa.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Saldoa.API.Infrastructure.Persistence;

namespace Saldoa.API.Auth;

public class RefreshTokenRepository(SaldoaDbContext dbContext) : IRefreshTokenRepository
{
    public Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default)
        => dbContext.Set<RefreshToken>().AddAsync(refreshToken, ct).AsTask();

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default)
        => dbContext.Set<RefreshToken>().SingleOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
}
