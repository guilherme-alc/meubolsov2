using MeuBolso.Application.Common.Exceptions;

namespace MeuBolso.Application.Categories.Exceptions;

public class CategoryAlreadyExistsException : DomainException
{
    public CategoryAlreadyExistsException(string name)
        : base($"Categoria '{name}' já existe") {}
}