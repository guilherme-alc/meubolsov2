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
            var amountsByBudgetPeriod = new Dictionary<(DateOnly Start, DateOnly End), decimal>();

            foreach (var installment in installments)
            {
                var budget = await _categoryBudgetRepository.GetActiveForPeriodAsync(
                    userId,
                    categoryId,
                    installment.Date,
                    ct);

                if (budget is null)
                    continue;

                var key = (budget.PeriodStart, budget.PeriodEnd);

                if (!amountsByBudgetPeriod.ContainsKey(key))
                    amountsByBudgetPeriod[key] = 0;

                amountsByBudgetPeriod[key] += installment.Amount;
            }

            foreach (var period in amountsByBudgetPeriod)
            {
                decimal spent = await _transactionRepository.GetTotalForPeriodExcludingAsync(
                        userId,
                        categoryId,
                        period.Key.Start,
                        period.Key.End,
                        excludeTransactionIds,
                        ct,
                        TransactionType.Expense
                );

                var totalProjected = spent + period.Value;

                var budget = await _categoryBudgetRepository.GetActiveForPeriodAsync(
                    userId,
                    categoryId,
                    period.Key.Start,
                    ct
                );

                if (budget is not null && totalProjected > budget.LimitAmount)
                {
                    result.Add(new BudgetAlert(
                        spent, 
                        totalProjected, 
                        budget.LimitAmount, 
                        $"Limite excedido para o período {period.Key.Start:MM/yyyy}"
                    ));
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
            var amountsByBudgetPeriod = new Dictionary<(DateOnly Start, DateOnly End), decimal>();

            foreach (var installment in installments)
            {
                var budget = await _categoryBudgetRepository.GetActiveForPeriodAsync(
                    userId,
                    categoryId,
                    installment.Date,
                    ct);

                if (budget is null)
                    continue;

                var key = (budget.PeriodStart, budget.PeriodEnd);

                if (!amountsByBudgetPeriod.ContainsKey(key))
                    amountsByBudgetPeriod[key] = 0;

                amountsByBudgetPeriod[key] += installment.Amount;
            }

            foreach (var period in amountsByBudgetPeriod)
            {
                decimal spent = await _transactionRepository.GetTotalForPeriodAsync(
                        userId,
                        categoryId,
                        period.Key.Start,
                        period.Key.End,
                        ct,
                        TransactionType.Expense
                );

                var totalProjected = spent + period.Value;

                var budget = await _categoryBudgetRepository.GetActiveForPeriodAsync(
                    userId,
                    categoryId,
                    period.Key.Start,
                    ct
                );

                if (budget is not null && totalProjected > budget.LimitAmount)
                {
                    result.Add(new BudgetAlert(
                        spent,
                        totalProjected,
                        budget.LimitAmount,
                        $"Limite excedido para o período {period.Key.Start:MM/yyyy}"
                    ));
                }
            }

            return result;
        }
    }
}
