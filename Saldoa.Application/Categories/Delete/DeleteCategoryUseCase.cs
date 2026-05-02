using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Categories.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;

namespace Saldoa.Application.Categories.Delete;

public class DeleteCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unit;

    public DeleteCategoryUseCase(ICategoryRepository categoryRepository, ITransactionRepository transactionRepository, IUnitOfWork unit)
    {
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
        _unit = unit;
    }

    public async Task<Result> ExecuteAsync(long id, string userId, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdForUpdateAsync(id, userId, ct);
        
        if (category is null)
        {
            var error = CategoryErrors.NotFound;
            return Result.Failure(error);
        }
        
        if (await _transactionRepository.ExistsForCategoryAsync(id, userId, ct))
        {
            var error = CategoryErrors.HasTransactions;
            return Result.Failure(error);
        }
        
        _categoryRepository.Remove(category);
        await _unit.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}