using FluentValidation;
using MeuBolso.Application.Auth.AuthDTO;

namespace MeuBolso.Application.Auth.Register;

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

        RuleFor(x => x.FullName)
            .MinimumLength(3)
            .WithMessage("O nome deve ser maior que 3 caracteres");
    }
}