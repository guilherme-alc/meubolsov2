using FluentValidation;

namespace Saldoa.Application.Auth.ConfirmEmail
{
    public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
    {
        public ConfirmEmailRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("O ID do usuário é obrigatório");

            RuleFor(x => x.EncodedToken)
                .NotEmpty()
                .WithMessage("O token de confirmação é obrigatório");
        }
    }
}