using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Common.Pagination;
using MeuBolso.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeuBolso.API.Persistence.Repositories;

public class CategoryRepository(MeuBolsoDbContext dbContext) : ICategoryRepository
{
    public async Task AddAsync(Category category, CancellationToken ct = default)
    {
        await dbContext.Categories.AddAsync(category, ct);
    }

    public void RemoveAsync(Category category)
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

    public async Task<bool> ExistsAsync(string userId, string name, CancellationToken ct = default)
    {
        var nameNormalized = name.Trim().ToUpperInvariant();
        
        return await dbContext.Categories.AnyAsync(
            c => c.UserId == userId && c.NormalizedName == nameNormalized,
            ct);
    }
}