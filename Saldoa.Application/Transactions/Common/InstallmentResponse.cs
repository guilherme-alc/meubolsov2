namespace Saldoa.Application.Transactions.Common
{
    public record InstallmentResponse(long Id, int InstallmentNumber, DateOnly PaidOrReceivedAt, decimal Amount);
}
