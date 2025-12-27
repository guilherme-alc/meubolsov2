using FluentValidation;

namespace MeuBolso.Application.Categories.Create;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome da categoria precisa ser fornecido")
            .MaximumLength(100)
            .WithMessage("O nome não pode ultrapassar 100 caracteres");        
        
        RuleFor(x => x.Description)
            .MaximumLength(255)
            .WithMessage("A descrição não pode ultrapassar 255 caracteres");
        
        RuleFor(x => x.Color)
            .MaximumLength(12)
            .WithMessage("A cor não pode ultrapassar 12 caracteres");
    }
}