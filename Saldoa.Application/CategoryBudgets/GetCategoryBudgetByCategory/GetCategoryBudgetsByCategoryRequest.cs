using Saldoa.Application.Common.Pagination;

namespace Saldoa.Application.CategoryBudgets.GetCategoryBudgetByCategory;

public sealed record GetCategoryBudgetsByCategoryRequest(
    int PageNumber = PaginationDefaults.PageNumber,
    int PageSize = PaginationDefaults.PageSize)
    : IPaginatedRequest;
