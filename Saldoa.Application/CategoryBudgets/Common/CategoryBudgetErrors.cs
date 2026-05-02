using Saldoa.Application.Common.Results;

namespace Saldoa.Application.CategoryBudgets.Common
{
    public static class CategoryBudgetErrors
    {
        public static Error NotFound =>
            new("CategoryBudget.NotFound", "Orçamento da categoria não encontrado", ErrorType.NotFound);
        public static Error AlreadyExists =>
            new("CategoryBudget.AlreadyExists", $"Já existe um orçamento para essa categoria no período informado", ErrorType.Conflict);
        public static Error ClosedPeriod =>
            new(
                "CategoryBudget.ClosedPeriod",
                "Não é possível alterar um limite de gasto já encerrado",
                ErrorType.Conflict
            );
    }
}
