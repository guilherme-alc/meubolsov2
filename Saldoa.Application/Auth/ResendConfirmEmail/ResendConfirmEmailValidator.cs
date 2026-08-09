using FluentValidation;

namespace Saldoa.Application.Auth.ResendConfirmEmail
{
    public class ResendConfirmEmailValidator : AbstractValidator<ResendConfirmEmailRequest>
    {
        public ResendConfirmEmailValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("O e-mail é obrigatório")
                .EmailAddress()
                .WithMessage("O e-mail é inválido");
        }
    }
}
