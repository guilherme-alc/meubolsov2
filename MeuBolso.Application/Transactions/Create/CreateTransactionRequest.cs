using MeuBolso.Application.Common.Requests;
using MeuBolso.Domain.Enums;

namespace MeuBolso.Application.Transactions.Create;

public class CreateTransactionRequest : AuthenticatedRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? PaidOrReceivedAt { get; set; }
    public ETransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public long CategoryId { get; set; }
}