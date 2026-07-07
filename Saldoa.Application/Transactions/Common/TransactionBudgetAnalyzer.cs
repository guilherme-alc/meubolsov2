using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.Common
{
    public sealed class TransactionBudgetAnalyzer
    {
        private readonly ICategoryBudgetRepository _categoryBudgetRepository;
        private readonly ITransactionRepository _transactionRepository;
        public TransactionBudgetAnalyzer(ICategoryBudgetRepository categoryBudgetRepository, ITransactionRepository transactionRepository)
        {
            _categoryBudgetRepository = categoryBudgetRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<List<BudgetAlert>> AnalyzeAsync(
            string userId,
            long categoryId,
            List<InstallmentDraft> installments,
            IReadOnlyCollection<long> excludeTransactionIds,
            CancellationToken ct)
        {
            var result = new List<BudgetAlert>();
            if (installments.Count == 0)
                return result;

            var installmentsStart = installments.Min(i => i.Date);
            var installmentsEnd = installments.Max(i => i.Date);

            var affectedBudgets = await _categoryBudgetRepository.GetActiveForPeriodAsync(
                userId,
                categoryId,
                installmentsStart,
                installmentsEnd,
                ct);

            if (affectedBudgets.Count == 0)
                return result;

            var budgetsStart = affectedBudgets.Min(b => b.PeriodStart);
            var budgetsEnd = affectedBudgets.Max(b => b.PeriodEnd);

            var totalsByDate = await _transactionRepository.GetTotalsByDateExcludingAsync(userId, categoryId, budgetsStart, budgetsEnd, excludeTransactionIds, ct, TransactionType.Expense);

            foreach (var budget in affectedBudgets)
            {
                var installmentsPerBudget = installments.Where(i => i.Date >= budget.PeriodStart && i.Date <= budget.PeriodEnd);

                if (installmentsPerBudget.Any())
                {
                    var amount = installmentsPerBudget.Sum(i => i.Amount);

                    var spent = totalsByDate.Where(t => t.Key >= budget.PeriodStart && t.Key <= budget.PeriodEnd).Sum(t => t.Value);

                    var totalProjected = spent + amount;

                    if (totalProjected > budget.LimitAmount)
                    {
                        result.Add(new BudgetAlert(
                            spent,
                            totalProjected,
                            budget.LimitAmount,
                            $"Limite excedido para o período {budget.PeriodStart:dd/MM/yyyy} - {budget.PeriodEnd:dd/MM/yyyy}"
                        ));
                    }
                }
            }

            return result;
        }

        public async Task<List<BudgetAlert>> AnalyzeAsync(
            string userId,
            long categoryId,
            List<InstallmentDraft> installments,
            CancellationToken ct)
        {
            var result = new List<BudgetAlert>();
            if (installments.Count == 0)
                return result;

            var installmentsStart = installments.Min(i => i.Date);
            var installmentsEnd = installments.Max(i => i.Date);

            var affectedBudgets = await _categoryBudgetRepository.GetActiveForPeriodAsync(
                userId,
                categoryId,
                installmentsStart,
                installmentsEnd,
                ct);

            if (affectedBudgets.Count == 0)
                return result;

            var budgetsStart = affectedBudgets.Min(b => b.PeriodStart);
            var budgetsEnd = affectedBudgets.Max(b => b.PeriodEnd);

            var totalsByDate = await _transactionRepository.GetTotalsByDateAsync(userId, categoryId, budgetsStart, budgetsEnd, ct, TransactionType.Expense);

            foreach (var budget in affectedBudgets)
            {
                var installmentsPerBudget = installments.Where(i => i.Date >= budget.PeriodStart && i.Date <= budget.PeriodEnd);

                if (installmentsPerBudget.Any())
                {
                    var amount = installmentsPerBudget.Sum(i => i.Amount);

                    var spent = totalsByDate.Where(t => t.Key >= budget.PeriodStart && t.Key <= budget.PeriodEnd).Sum(t => t.Value);

                    var totalProjected = spent + amount;

                    if (totalProjected > budget.LimitAmount)
                    {
                        result.Add(new BudgetAlert(
                            spent,
                            totalProjected,
                            budget.LimitAmount,
                            $"Limite excedido para o período {budget.PeriodStart:dd/MM/yyyy} - {budget.PeriodEnd:dd/MM/yyyy}"
                        ));
                    }
                }
            }

            return result;
        }
    }
}
