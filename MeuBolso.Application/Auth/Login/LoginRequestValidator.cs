using FluentValidation;

namespace MeuBolso.Application.Auth.Login;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail e senha são obrigatórios")
            .EmailAddress().WithMessage("E-mail e senha são obrigatórios");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("E-mail e senha são obrigatórios");
    }
}
