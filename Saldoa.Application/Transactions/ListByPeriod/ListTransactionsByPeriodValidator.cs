using FluentValidation;
using System.Globalization;

namespace Saldoa.Application.Transactions.ListByPeriod;

public class ListTransactionsByPeriodValidator : AbstractValidator<ListTransactionsByPeriodRequest>
{
    public ListTransactionsByPeriodValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);
        
        RuleFor(x => x)
            .Must(x =>
                !x.StartDate.HasValue ||
                !x.EndDate.HasValue ||
                x.EndDate >= x.StartDate)
            .WithMessage("A data final não pode ser menor que a data inicial.");

        When(x => x.CategoryId is not null, () =>
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("Id da categoria inválido");
        });
        
        When(x => x.Type is not null, () =>
        {
            RuleFor(x => x.Type!.Value)
                .IsInEnum()
                .WithMessage("Tipo da transação inválido.");
        });

        When(x => x.YearMonth is not null, () =>
        {
            RuleFor(x => x.YearMonth)
                .Must(BeValidMonth)
                .WithMessage("O parâmetro month deve estar no formato yyyy-MM. Exemplo: 2026-06");
        });

        RuleFor(x => x)
            .Must(x => string.IsNullOrWhiteSpace(x.YearMonth) ||
                       (!x.StartDate.HasValue && !x.EndDate.HasValue))
            .WithMessage("Use YearMonth ou StartDate/EndDate, não ambos.");
    }

    private static bool BeValidMonth(string? month)
    {
        return DateOnly.TryParseExact(
            month + "-01",
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }
}