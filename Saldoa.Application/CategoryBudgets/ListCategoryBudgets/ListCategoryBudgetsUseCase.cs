using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.CategoryBudgets.Common;
using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.CategoryBudgets.ListCategoryBudgets;

public class ListCategoryBudgetsUseCase
{
    private readonly ICategoryBudgetRepository _categoryBudgetRepository;
    public ListCategoryBudgetsUseCase(ICategoryBudgetRepository categoryBudgetRepository)
    {
        _categoryBudgetRepository = categoryBudgetRepository;
    }

    public async Task<Result<PagedResult<CategoryBudgetResponse>>> ExecuteAsync(string userId, ListCategoryBudgetsRequest request,
        CancellationToken ct)
    {
        var data = await _categoryBudgetRepository.ListAsync(request.PageNumber,
            request.PageSize,
            userId,
            request.StartDate,
            request.EndDate,
            request.Active,
            ct);

        var categoryBudgetsResponse = data.Items.Select(c =>
            new CategoryBudgetResponse(
                c.Id,
                c.CategoryId,
                c.PeriodStart,
                c.PeriodEnd,
                c.LimitAmount
            )
        ).ToList();

        var result = new PagedResult<CategoryBudgetResponse>(categoryBudgetsResponse, data.TotalCount, data.PageNumber, data.PageSize);
        
        return Result<PagedResult<CategoryBudgetResponse>>.Success(result);
    }
}