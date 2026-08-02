using FluentValidation;

namespace Saldoa.Application.Auth.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("O e-mail é obrigatório")
            .EmailAddress()
            .WithMessage("O e-mail precisa ser válido");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("A senha é obrigatória")
            .MinimumLength(8)
            .WithMessage("A senha precisa ter no mínimo 8 caracteres");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("O nome é obrigatório")
            .MinimumLength(2)
            .WithMessage("O nome deve ter no mínimo 2 caracteres")
            .MaximumLength(100)
            .WithMessage("O nome não pode ter mais de 100 caracteres");

        RuleFor(x => x.LastName)
            .MinimumLength(2)
            .WithMessage("O sobrenome deve ter no mínimo 2 caracteres")
            .MaximumLength(100)
            .WithMessage("O sobrenome não pode ter mais de 100 caracteres");
    }
}