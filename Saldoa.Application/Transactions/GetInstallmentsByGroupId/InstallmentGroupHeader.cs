using Saldoa.Application.Transactions.Common;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.GetInstallmentsByGroupId
{
    public sealed record InstallmentGroupHeader(
        Guid InstallmentGroupId,
        string Title,
        string? Description,
        TransactionType Type,
        decimal TotalAmount,
        int TotalInstallments,
        CategorySummaryResponse Category
    );
}
