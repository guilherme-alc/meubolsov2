using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Transactions.GetInstallmentsByGroupId;
using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.Abstractions;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction, CancellationToken ct);
    Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken ct);
    void Delete(Transaction transaction);
    Task DeleteByInstallmentGroupId(Guid installmentGroupId, CancellationToken ct);
    Task<Transaction?> GetByIdAsync(long id, string userId, CancellationToken ct);
    Task<Transaction?> GetByIdForUpdateAsync(long id, string userId, CancellationToken ct);
    Task<Transaction?> GetByIdWithCategoryAsync(long id, string userId, CancellationToken ct);
    Task<List<Transaction>?> GetInstallmentsForUpdateAsync(Guid installmentGroupId, string userId, CancellationToken ct);
    Task<InstallmentGroupHeader?> GetInstallmentGroupHeaderAsync(Guid installmentGroupId, string userId, CancellationToken ct);
    Task<PagedResult<Transaction>> GetInstallmentsByGroupIdAsync(Guid installmentGroupId, string userId, int pageNumber, int pageSize, CancellationToken ct);
    Task<PagedResult<Transaction>> ListByPeriodAsync(string userId, DateOnly startDate, DateOnly endDate, TransactionType? type, int pageNumber, int pageSize, CancellationToken ct);
    Task<PagedResult<Transaction>> ListByCategoryAsync(string userId, DateOnly startDate, DateOnly endDate, long categoryId, int pageNumber, int pageSize, CancellationToken ct);
    Task<decimal> GetTotalForPeriodAsync(string userId, long categoryId, DateOnly start, DateOnly end, CancellationToken ct, TransactionType? type = TransactionType.Expense);
    Task<decimal> GetTotalForPeriodExcludingAsync(string userId, long categoryId, DateOnly start, DateOnly end, IReadOnlyCollection<long> excludeTransactionIds, CancellationToken ct, TransactionType? type = TransactionType.Expense);
    Task<bool> ExistsForCategoryAsync(long categoryId, string userId, CancellationToken ct);
}