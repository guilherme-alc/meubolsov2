using FluentValidation;
using Saldoa.Application.Common.Pagination;

namespace Saldoa.Application.Categories.List;

public sealed class ListCategoriesValidator : AbstractValidator<ListCategoriesRequest>
{
    public ListCategoriesValidator()
    {
        this.AddPaginationRules();
    }
}