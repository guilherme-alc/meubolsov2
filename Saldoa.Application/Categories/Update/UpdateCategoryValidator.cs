using FluentValidation;

namespace Saldoa.Application.Categories.Update;

public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryValidator()
    {
        When(x => x.Name is not null, () =>
        {
            RuleFor(x => x.Name!)
                .Must(n => !string.IsNullOrWhiteSpace(n))
                .WithMessage("O nome da categoria precisa ser fornecido")
                .MaximumLength(100)
                .WithMessage("O nome não pode ultrapassar 100 caracteres");
        });

        When(x => x.Description is not null, () =>
        {
            RuleFor(x => x.Description!)
                .MaximumLength(255)
                .WithMessage("A descrição não pode ultrapassar 255 caracteres");
        });

        When(x => x.Color is not null, () =>
        {
            RuleFor(x => x.Color!)
                .MaximumLength(12)
                .WithMessage("A cor não pode ultrapassar 12 caracteres");
        });
    }
}