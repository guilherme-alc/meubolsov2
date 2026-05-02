namespace Saldoa.Application.Transactions.ListByCategory;

public sealed record ListTransactionsByCategoryRequest(
    DateOnly? StartDate, 
    DateOnly? EndDate, 
    long CategoryId,
    int PageNumber = 1,
    int PageSize = 20);