using Saldoa.Domain.Enums;
using Saldoa.Domain.ValueObjects;

namespace Saldoa.Application.Transactions.Common;

public record CategorySummaryResponse(long Id, string Name, string? Color);

public record TransactionResponse(
    long Id,
    string Title,
    string? Description,
    TransactionType Type,
    decimal Amount,
    DateOnly? PaidOrReceivedAt,
    CategorySummaryResponse Category,
    InstallmentInfo? InstallmentInfo
);