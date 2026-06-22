using Saldoa.Application.Transactions.Common;

namespace Saldoa.Application.Transactions.Update
{
    public record UpdateTransactionResponse(IEnumerable<BudgetAlert>? BudgetAlerts);
}
