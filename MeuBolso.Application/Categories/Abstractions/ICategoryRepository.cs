using MeuBolso.Application.Common.Pagination;
using MeuBolso.Domain.Entities;

namespace MeuBolso.Application.Categories.Abstractions;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    void RemoveAsync(Category category);
    Task<Category?> GetByIdAsync(long id, string userId);
    Task<Category?> GetByIdForUpdateAsync(long id, string userId);
    Task<PagedResult<Category>> ListAsync(int pageNumber, int pageSize, string userId); 
    Task<bool> ExistsAsync(string userId, string name);
}