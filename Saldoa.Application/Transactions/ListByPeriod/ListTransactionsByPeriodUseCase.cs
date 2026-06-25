using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;

namespace Saldoa.Application.Transactions.ListByPeriod;

public class ListTransactionsByPeriodUseCase
{
    private readonly ITransactionRepository _transactionRepository;

    public ListTransactionsByPeriodUseCase(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }
    
    public async Task<Result<PagedResult<TransactionResponse>>> ExecuteAsync(string userId, ListTransactionsByPeriodRequest request, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var startDate = request.StartDate ?? today.AddYears(-1);
        var endDate   = request.EndDate ?? today;
        
        var data = await _transactionRepository.ListByPeriodAsync(
            userId, 
            startDate, 
            endDate,
            request.Type,
            request.CategoryId,
            request.PageNumber!, 
            request.PageSize!, 
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