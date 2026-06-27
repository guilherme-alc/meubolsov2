using Saldoa.Application.Common.Pagination;

namespace Saldoa.Application.CategoryBudgets.ListCategoryBudgets;

public sealed record ListCategoryBudgetsRequest(DateOnly? StartDate, DateOnly? EndDate, bool? Active) : PaginatedRequest;