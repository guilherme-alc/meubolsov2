using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;

namespace Saldoa.Application.Transactions.GetById;

public class GetTransactionByIdUseCase
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionByIdUseCase(ITransactionRepository transactionRepository)
    {
        _transactionRepository =  transactionRepository;
    }
    
    public async Task<Result<TransactionResponse>> ExecuteAsync(long id, string userId, CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetByIdWithCategoryAsync(id, userId, ct);

        if (transaction is null)
        {
            var error = TransactionErrors.NotFound;
            return Result<TransactionResponse>.Failure(error);
        }

        return Result<TransactionResponse>.Success(new TransactionResponse(
            transaction.Id,
            transaction.Title,
            transaction.Description,
            transaction.Type,
            transaction.Amount,
            transaction.PaidOrReceivedAt,
            new CategorySummaryResponse(
                transaction.Category.Id,
                transaction.Category.Name,
                transaction.Category.Color),
            transaction.InstallmentInfo
        ));
    }
}