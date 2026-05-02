using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Transactions.Common
{
    public static class TransactionErrors
    {
        public static Error NotFound =>
            new("Transaction.NotFound", "Transação não encontrada", ErrorType.NotFound);
        public static Error InstallmentNotFound =>
            new("Transaction.NotFound", "Parcela não encontrada", ErrorType.NotFound);
    }
}
