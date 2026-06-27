namespace Saldoa.Application.Common.Pagination;

public abstract record PaginatedRequest
{
    public int PageNumber { get; init; } = PaginationDefaults.PageNumber;
    public int PageSize { get; init; } = PaginationDefaults.PageSize;
}
