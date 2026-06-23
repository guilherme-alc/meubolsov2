using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Transactions.Common;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.GetInstallmentsByGroupId
{
    public record GetInstallmentsByGroupIdResponse(
        Guid InstallmentGroupId,
        string Title,
        string? Description,
        TransactionType Type,
        decimal TotalAmount,
        int TotalInstallments,
        CategorySummaryResponse Category,
        PagedResult<InstallmentResponse> Installments
    );
}
