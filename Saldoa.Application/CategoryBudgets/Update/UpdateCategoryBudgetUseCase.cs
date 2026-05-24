using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.CategoryBudgets.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.CategoryBudgets.Update;

public class UpdateCategoryBudgetUseCase
{
    private readonly ICategoryBudgetRepository _categoryBudgetRepository;
    private readonly IUnitOfWork _unit;

    public UpdateCategoryBudgetUseCase(ICategoryBudgetRepository categoryBudgetRepository, IUnitOfWork unit)
    {
        _categoryBudgetRepository = categoryBudgetRepository;
        _unit = unit;
    }

    public async Task<Result> ExecuteAsync(string userId, long categoryBudgetId, UpdateCategoryBudgetRequest request, CancellationToken ct)
    {
        var categoryBudget = await _categoryBudgetRepository.GetByIdForUpdateAsync(categoryBudgetId, userId, ct);
        
        if (categoryBudget == null)
        {
            var error = CategoryBudgetErrors.NotFound;
            return Result.Failure(error);
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if(categoryBudget.PeriodEnd < today)
        {
            var error = CategoryBudgetErrors.ClosedPeriod;
            return Result.Failure(error);
        }

        if (request.PeriodStart != categoryBudget.PeriodStart || request.PeriodEnd != categoryBudget.PeriodEnd)
        {
            var exists = await _categoryBudgetRepository.ExistsForPeriodAsync(
                userId,
                categoryBudget.CategoryId,
                categoryBudget.Id,
                request.PeriodStart,
                request.PeriodEnd,
                ct);

            if (exists)
            {
                var error = CategoryBudgetErrors.AlreadyExists;
                return Result.Failure(error);
            }

            categoryBudget.SetPeriod(request.PeriodStart, request.PeriodEnd);
        }
        
        categoryBudget.SetLimitAmount(request.LimitAmount);
        
        await _unit.SaveChangesAsync(ct);
        return Result.Success();
    }
}