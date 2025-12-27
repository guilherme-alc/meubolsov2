using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Common.Abstractions;
using MeuBolso.Application.Transactions.Abstractions;
using MeuBolso.Infrastructure.Categories;
using MeuBolso.Infrastructure.Transactions;
using Microsoft.EntityFrameworkCore.Storage;

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