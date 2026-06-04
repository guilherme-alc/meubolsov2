using Microsoft.EntityFrameworkCore;
using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Common.Pagination;
using Saldoa.Domain.Entities;

namespace Saldoa.API.Infrastructure.Persistence.Repositories;

public class CategoryRepository(SaldoaDbContext dbContext) : ICategoryRepository
{
    public async Task AddAsync(Category category, CancellationToken ct = default)
    {
        await dbContext.Categories.AddAsync(category, ct);
    }

    public void Remove(Category category)
    {
        dbContext.Categories.Remove(category);
    }

    public async Task<Category?> GetByIdAsync(long id, string userId, CancellationToken ct = default)
    {
        var category = await dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, ct);

        return category;
    }
    
    public async Task<Category?> GetByIdForUpdateAsync(long id, string userId, CancellationToken ct = default)
    {
        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, ct);

        return category;
    }

    public async Task<PagedResult<Category>> ListAsync(
        int pageNumber, 
        int pageSize,
        string userId,
        CancellationToken ct = default)
    {
        var query = dbContext
            .Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId);

        var total = await query.CountAsync(ct);
        
        var data = await query
            .OrderBy(c => c.Name) // ou CreatedAt, ou Id
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Category>(data, total, pageNumber, pageSize);
    }

    public Task<bool> ExistsAsync(string userId, string name, CancellationToken ct = default)
    {
        var nameNormalized = name.Trim().ToUpperInvariant();
        
        return dbContext.Categories.AnyAsync(
            c => c.UserId == userId && c.NormalizedName == nameNormalized,
            ct);
    }
}