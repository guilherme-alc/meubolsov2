using FluentValidation;

namespace Saldoa.Application.Common.Pagination;

public static class PaginationValidationExtensions
{
    public static void AddPaginationRules<T>(this AbstractValidator<T> validator) 
        where T : IPaginatedRequest
    {
        validator.RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber deve ser maior que zero.");

        validator.RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PaginationDefaults.MaxPageSize)
            .WithMessage($"PageSize deve estar entre 1 e {PaginationDefaults.MaxPageSize}.");
    }
}
