using FluentValidation;

namespace Saldoa.Application.Auth.SendPasswordReset
{
    public class SendPasswordResetValidatorToken : AbstractValidator<SendPasswordResetTokenRequest>
    {
        public SendPasswordResetValidatorToken()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("O e-mail é obrigatório")
                .EmailAddress()
                .WithMessage("O e-mail é inválido");
        }
    }
}
