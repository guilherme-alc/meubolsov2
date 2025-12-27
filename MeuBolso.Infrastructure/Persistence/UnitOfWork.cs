using MeuBolso.Application.Common.Abstractions;

namespace MeuBolso.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    readonly MeuBolsoDbContext _dbContext;

    public UnitOfWork(MeuBolsoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task SaveChangesAsync(
        CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}