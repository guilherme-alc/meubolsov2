using FluentValidation;
using Saldoa.Application.Common.Pagination;

namespace Saldoa.Application.CategoryBudgets.GetCategoryBudgetByCategory;

public class GetCategoryBudgetsByCategoryValidator : AbstractValidator<GetCategoryBudgetsByCategoryRequest>
{
    public GetCategoryBudgetsByCategoryValidator()
    {
        this.AddPaginationRules();
    }
}