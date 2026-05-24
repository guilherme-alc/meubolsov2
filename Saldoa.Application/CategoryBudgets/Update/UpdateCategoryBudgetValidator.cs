using FluentValidation;

namespace Saldoa.Application.CategoryBudgets.Update;

public class UpdateCategoryBudgetValidator : AbstractValidator<UpdateCategoryBudgetRequest>
{
    public UpdateCategoryBudgetValidator()
    {
        RuleFor(x => x.LimitAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Limite não pode ser negativo");

        RuleFor(x => x)
            .Must(x =>
                x.PeriodEnd >= x.PeriodStart)
            .WithMessage("A data final não pode ser menor que a data inicial.");
    }
}