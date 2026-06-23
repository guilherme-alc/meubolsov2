using Microsoft.EntityFrameworkCore;
using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;
using Saldoa.Application.Transactions.GetInstallmentsByGroupId;
using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;

namespace Saldoa.Infrastructure.Persistence.Repositories;

public class TransactionRepository(SaldoaDbContext dbContext) : ITransactionRepository
{
    public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        await dbContext.Transactions.AddAsync(transaction, ct);
    }

    public async Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken ct = default)
    {
        await dbContext.Transactions.AddRangeAsync(transactions, ct);
    }

    public void Delete(Transaction transaction)
    {
        dbContext.Transactions.Remove(transaction);
    }

    public async Task DeleteByInstallmentGroupId(Guid installmentGroupId, CancellationToken ct = default)
    {
        var transaction = await dbContext.Transactions
            .Where(t => t.InstallmentInfo.InstallmentGroupId == installmentGroupId)
            .ToListAsync(ct);

        dbContext.RemoveRange(transaction);
    }

    public async Task<Transaction?> GetByIdAsync(long id, string userId, CancellationToken ct = default)
    {
        return await dbContext.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);
    }

    public async Task<Transaction?> GetByIdForUpdateAsync(long id, string userId, CancellationToken ct = default)
    {
        return await dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);
    }

    public async Task<Transaction?> GetByIdWithCategoryAsync(long id, string userId, CancellationToken ct)
    {
        return await dbContext.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);
    }
    
    public async Task<PagedResult<Transaction>> ListByPeriodAsync(
        string userId,
        DateOnly startDate,
        DateOnly endDate,
        TransactionType? type,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = dbContext
            .Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        t.PaidOrReceivedAt >= startDate &&
                        t.PaidOrReceivedAt <= endDate &&
                        (type == null || t.Type == type));

        var total = await query.CountAsync(ct);

        var data = await query
            .Include(t => t.Category)
            .OrderByDescending(t => t.PaidOrReceivedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Transaction>(data, total, pageNumber, pageSize);
    }

    public async Task<PagedResult<Transaction>> ListByCategoryAsync(string userId, DateOnly startDate, DateOnly endDate,
        long categoryId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var query = dbContext
            .Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        t.PaidOrReceivedAt >= startDate &&
                        t.PaidOrReceivedAt <= endDate &&
                        t.CategoryId == categoryId
            );

        var total = await query.CountAsync(ct);

        var data = await query
            .Include(t => t.Category)
            .OrderByDescending(t => t.PaidOrReceivedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Transaction>(data, total, pageNumber, pageSize);
    }

    public async Task<decimal> GetTotalForPeriodAsync(
        string userId,
        long categoryId,
        DateOnly start,
        DateOnly end,
        CancellationToken ct,
        TransactionType? type = TransactionType.Expense)
    {
        return await dbContext.Transactions
            .Where(t =>
                t.UserId == userId &&
                t.CategoryId == categoryId &&
                t.PaidOrReceivedAt >= start &&
                t.PaidOrReceivedAt <= end  &&
                t.Type == type)
            .SumAsync(t => t.Amount, ct);
    }

    public async Task<decimal> GetTotalForPeriodExcludingAsync(
        string userId,
        long categoryId,
        DateOnly start,
        DateOnly end,
        IReadOnlyCollection<long> excludeTransactionIds,
        CancellationToken ct,
        TransactionType? type = TransactionType.Expense)
    {
        return await dbContext.Transactions
            .Where(t =>
                t.UserId == userId &&
                t.CategoryId == categoryId &&
                !excludeTransactionIds.Contains(t.Id) &&
                t.PaidOrReceivedAt >= start &&
                t.PaidOrReceivedAt <= end &&
                t.Type == type)
            .SumAsync(t => t.Amount, ct);
    }

    public async Task<bool> ExistsForCategoryAsync(long categoryId, string userId, CancellationToken ct)
    {
        return await dbContext.Transactions
            .AsNoTracking()
            .AnyAsync(
                t => t.CategoryId == categoryId && 
                     t.UserId == userId,
                ct);
    }

    public async Task<List<Transaction>?> GetInstallmentsForUpdateAsync(Guid installmentGroupId, string userId, CancellationToken ct)
    {
        return await dbContext.Transactions
             .Where(t => t.InstallmentInfo.InstallmentGroupId == installmentGroupId && t.UserId == userId)
             .ToListAsync(ct);
    }

    public async Task<PagedResult<Transaction>> GetInstallmentsByGroupIdAsync(Guid installmentGroupId, string userId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var query = dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.InstallmentInfo.InstallmentGroupId == installmentGroupId && t.UserId == userId);

        var total = await query.CountAsync(ct);

        var data = await query
            .OrderBy(t => t.InstallmentInfo.InstallmentNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Transaction>(data, total, pageNumber, pageSize);
    }

    public async Task<InstallmentGroupHeader?> GetInstallmentGroupHeaderAsync(Guid installmentGroupId, string userId, CancellationToken ct)
    {

        var query = dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.InstallmentInfo.InstallmentGroupId == installmentGroupId && 
                t.UserId == userId);

        var header = await query
            .Include(t => t.Category)
            .OrderBy(t => t.InstallmentInfo.InstallmentNumber)
            .Select(t => new
            {
                t.Title,
                t.Description,
                t.Type,
                t.InstallmentInfo.TotalInstallments,
                Category = new CategorySummaryResponse(
                    t.Category.Id,
                    t.Category.Name,
                    t.Category.Color
                )
            })
            .FirstOrDefaultAsync(ct);

        if (header is null)
            return null;

        var totalAmount = await query.SumAsync(t => t.Amount, ct);

        return new InstallmentGroupHeader(
            installmentGroupId,
            header.Title,
            header.Description,
            header.Type,
            totalAmount,
            header.TotalInstallments,
            header.Category
        );
    }
}