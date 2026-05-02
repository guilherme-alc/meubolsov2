using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Categories.Common
{
    public static class CategoryErrors
    {
        public static Error NotFound =>
            new("Category.NotFound", "Categoria não encontrada", ErrorType.NotFound);
        public static Error AlreadyExists(string name) =>
            new("Category.AlreadyExists", $"Uma categoria com o nome {name} já existe", ErrorType.Conflict);
        public static Error HasTransactions =>
            new("Category.HasTransactions", "Não é possível excluir uma categoria com transações", ErrorType.Conflict);
    }
}
