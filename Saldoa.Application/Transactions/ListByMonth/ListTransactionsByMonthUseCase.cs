using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;

namespace Saldoa.Application.Transactions.ListByMonth;

public class ListTransactionsByMonthUseCase
{
    private readonly ITransactionRepository _transactionRepository;

    public ListTransactionsByMonthUseCase(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }
    
    public async Task<Result<PagedResult<TransactionResponse>>> ExecuteAsync(string userId, ListTransactionsByMonthRequest request, CancellationToken ct)
    {
        var startDate = new DateOnly(request.Year, request.Month, 1);
        var endDate   = startDate.AddMonths(1).AddDays(-1);
        
        var data = await _transactionRepository.ListByPeriodAsync(
            userId, 
            startDate, 
            endDate,
            request.Type,
            request.CategoryId,
            request.PageNumber, 
            request.PageSize, 
            ct);

        var transactionsResponse = data.Items.Select(t =>
                new TransactionResponse(
                    t.Id, 
                    t.Title, 
                    t.Description, 
                    t.Type, 
                    t.Amount, 
                    t.PaidOrReceivedAt, 
                    new CategorySummaryResponse(t.Category.Id, t.Category.Name, t.Category.Color),
                    t.InstallmentInfo
                )
            )
            .ToList();

        var result =
            new PagedResult<TransactionResponse>(transactionsResponse, data.TotalCount, data.PageNumber, data.PageSize);
        
        return Result<PagedResult<TransactionResponse>>.Success(result);
    }
}