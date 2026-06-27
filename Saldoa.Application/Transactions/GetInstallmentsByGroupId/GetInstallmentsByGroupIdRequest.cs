using Saldoa.Application.Common.Pagination;

namespace Saldoa.Application.Transactions.GetInstallmentsByGroupId;

public sealed record GetInstallmentsByGroupIdRequest(
    int PageNumber = PaginationDefaults.PageNumber,
    int PageSize = PaginationDefaults.PageSize)
    : IPaginatedRequest;
