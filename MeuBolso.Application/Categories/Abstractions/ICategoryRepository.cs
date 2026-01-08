using MeuBolso.Application.Common.Pagination;
using MeuBolso.Domain.Entities;

namespace MeuBolso.Application.Categories.Abstractions;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken ct);
    void RemoveAsync(Category category);
    Task<Category?> GetByIdAsync(long id, string userId, CancellationToken ct);
    Task<Category?> GetByIdForUpdateAsync(long id, string userId, CancellationToken ct);
    Task<PagedResult<Category>> ListAsync(int pageNumber, int pageSize, string userId, CancellationToken ct); 
    Task<bool> ExistsAsync(string userId, string name, CancellationToken ct);
}