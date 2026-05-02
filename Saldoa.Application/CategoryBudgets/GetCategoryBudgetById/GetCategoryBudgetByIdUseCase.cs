using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.CategoryBudgets.Common;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;

namespace Saldoa.Application.CategoryBudgets.GetCategoryBudgetById;

public class GetCategoryBudgetByIdUseCase
{
    private readonly ICategoryBudgetRepository _categoryBudgetRepository;
    private readonly ITransactionRepository _transactionRepository;

    public GetCategoryBudgetByIdUseCase(ICategoryBudgetRepository categoryBudgetRepository, ITransactionRepository transactionRepository)
    {
        _categoryBudgetRepository = categoryBudgetRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<CategoryBudgetDetailsResponse>> ExecuteAsync(string userId, long categoryBudgetId, CancellationToken ct)
    {
        var categoryBudget = await _categoryBudgetRepository.GetByIdAsync(categoryBudgetId, userId, ct);
        
        if(categoryBudget is null)
        {
            var error = CategoryBudgetErrors.NotFound;
            return Result<CategoryBudgetDetailsResponse>.Failure(error);
        }

        var total = await _transactionRepository.GetTotalForPeriodAsync(
            userId, 
            categoryBudget.CategoryId, 
            categoryBudget.PeriodStart, 
            categoryBudget.PeriodEnd, 
            ct);
        
        var totalSpent = total;
        
        var remainingAmount = categoryBudget.LimitAmount - totalSpent;
        
        var percentageUsed = categoryBudget.LimitAmount == 0
            ? 0
            : (totalSpent / categoryBudget.LimitAmount) * 100;
        
        var response = new CategoryBudgetDetailsResponse(
            categoryBudget.Id, 
            categoryBudget.CategoryId, 
            categoryBudget.PeriodStart, 
            categoryBudget.PeriodEnd, 
            categoryBudget.LimitAmount,
            totalSpent,
            remainingAmount,
            percentageUsed);
        
        return Result<CategoryBudgetDetailsResponse>.Success(response);
    } 
}