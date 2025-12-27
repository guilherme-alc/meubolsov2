using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Common.Pagination;
using MeuBolso.Domain.Entities;
using MeuBolso.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MeuBolso.Infrastructure.Categories;

public class CategoryRepository : ICategoryRepository
{
    private readonly MeuBolsoDbContext _dbContext;
    public CategoryRepository(MeuBolsoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(Category category)
    {
        await _dbContext.Categories.AddAsync(category);
    }

    public void RemoveAsync(Category category)
    {
        _dbContext.Categories.Remove(category);
    }

    public async Task<Category?> GetByIdAsync(long id, string userId)
    {
        var category = await _dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        return category;
    }
    
    public async Task<Category?> GetByIdForUpdateAsync(long id, string userId)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        return category;
    }

    public async Task<PagedResult<Category>> ListAsync(
        int pageNumber, 
        int pageSize,
        string userId)
    {
        var query = _dbContext
            .Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId);

        var total = await query.CountAsync();
        
        var data = await query
            .OrderBy(c => c.Name) // ou CreatedAt, ou Id
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Category>(data, total, pageNumber, pageSize);;
    }

    public async Task<bool> ExistsAsync(string userId, string name)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Name == name && c.UserId == userId);
    }
}