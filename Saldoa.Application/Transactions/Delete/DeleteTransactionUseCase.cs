using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;

namespace Saldoa.Application.Transactions.Delete;

public class DeleteTransactionUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unit;
    
    public DeleteTransactionUseCase(ITransactionRepository transactionRepository, IUnitOfWork unit)
    {
        _transactionRepository = transactionRepository;
        _unit = unit;
    }

    public async Task<Result> ExecuteAsync(long id, string userId, CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetByIdForUpdateAsync(id, userId, ct);
        if (transaction is null)
        {
            var error = TransactionErrors.NotFound;
            return Result.Failure(error);
        }
        
        if(transaction.InstallmentInfo.InstallmentGroupId == null)
            _transactionRepository.Delete(transaction);
        else
           await _transactionRepository.DeleteByInstallmentGroupId(transaction.InstallmentInfo.InstallmentGroupId.Value, ct);

        await _unit.SaveChangesAsync(ct);
        
        return Result.Success();
    } 
}