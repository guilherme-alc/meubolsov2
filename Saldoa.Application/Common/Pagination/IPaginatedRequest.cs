namespace Saldoa.Application.Common.Pagination;

public interface IPaginatedRequest
{
    int PageNumber { get; }
    int PageSize { get; }
}
