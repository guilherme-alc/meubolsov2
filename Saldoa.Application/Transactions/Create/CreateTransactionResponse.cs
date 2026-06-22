using Saldoa.Application.Transactions.Common;

namespace Saldoa.Application.Transactions.Create
{
    public record CreateTransactionsResponse(IEnumerable<TransactionResponse> Transactions, IEnumerable<BudgetAlert>? BudgetAlerts);
}
