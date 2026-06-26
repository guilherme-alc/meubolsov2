using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.ListByPeriod;

public sealed record ListTransactionsByPeriodRequest(
    string? YearMonth,
    DateOnly? StartDate, 
    DateOnly? EndDate,
    long? CategoryId,
    TransactionType? Type, 
    int PageNumber = 1, 
    int PageSize = 20);