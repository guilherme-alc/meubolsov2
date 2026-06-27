using Saldoa.Application.Common.Pagination;

namespace Saldoa.Application.Categories.List;

public sealed record ListCategoriesRequest(
    int PageNumber = PaginationDefaults.PageNumber,
    int PageSize = PaginationDefaults.PageSize)
    : IPaginatedRequest;
