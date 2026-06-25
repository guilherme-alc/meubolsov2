using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.ListByMonth;

public record ListTransactionsByMonthRequest(
    int Year, 
    int Month, 
    TransactionType? Type, 
    long CategoryId,
    int PageNumber = 1, 
    int PageSize = 20);