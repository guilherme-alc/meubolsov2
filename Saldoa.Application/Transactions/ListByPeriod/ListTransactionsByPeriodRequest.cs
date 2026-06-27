using Saldoa.Application.Common.Pagination;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.ListByPeriod;

public sealed record ListTransactionsByPeriodRequest(
    string? YearMonth,
    DateOnly? StartDate,
    DateOnly? EndDate,
    long? CategoryId,
    TransactionType? Type)
    : PaginatedRequest;