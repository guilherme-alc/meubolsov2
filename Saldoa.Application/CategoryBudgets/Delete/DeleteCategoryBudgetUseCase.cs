using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.CategoryBudgets.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.CategoryBudgets.Delete;

public class DeleteCategoryBudgetUseCase
{
    private readonly ICategoryBudgetRepository _categoryBudgetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryBudgetUseCase(ICategoryBudgetRepository categoryBudgetRepository, IUnitOfWork unitOfWork)
    {
        _categoryBudgetRepository = categoryBudgetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(string userId, long categoryBudgetId, CancellationToken ct)
    {
        var categoryBudget = await _categoryBudgetRepository.GetByIdForUpdateAsync(categoryBudgetId, userId, ct);
        
        if(categoryBudget is null)
        {
            var error = CategoryBudgetErrors.NotFound;
            return Result.Failure(error);
        }
        
        _categoryBudgetRepository.Remove(categoryBudget);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}