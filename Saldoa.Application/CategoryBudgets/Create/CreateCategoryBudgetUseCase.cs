using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.CategoryBudgets.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.CategoryBudgets.Create;

public class CreateCategoryBudgetUseCase
{
    private readonly ICategoryBudgetRepository _categoryBudgetRepository;
    private readonly IUnitOfWork _unit;

    public CreateCategoryBudgetUseCase(ICategoryBudgetRepository categoryBudgetRepository, IUnitOfWork unit)
    {
        _categoryBudgetRepository = categoryBudgetRepository;
        _unit = unit;
    }
    
    public async Task<Result<CategoryBudgetResponse>> ExecuteAsync(string userId, CreateCategoryBudgetRequest request, CancellationToken ct)
    {
        if (await _categoryBudgetRepository.ExistsForPeriodAsync(
                userId, 
                request.CategoryId, 
                request.PeriodStart,
                request.PeriodEnd, ct))
        {
            var error = CategoryBudgetErrors.AlreadyExists;
            return Result<CategoryBudgetResponse>.Failure(error);
        }

        var categoryBudget = new CategoryBudget(request.CategoryId, request.PeriodStart, request.PeriodEnd, request.LimitAmount, userId);

        await _categoryBudgetRepository.AddAsync(categoryBudget, ct);
        await _unit.SaveChangesAsync(ct);

        return Result<CategoryBudgetResponse>.Success(new CategoryBudgetResponse(
            categoryBudget.Id,
            categoryBudget.CategoryId,
            categoryBudget.PeriodStart,
            categoryBudget.PeriodEnd,
            categoryBudget.LimitAmount));
    }
}