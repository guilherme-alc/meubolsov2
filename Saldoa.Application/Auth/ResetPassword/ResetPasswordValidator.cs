using FluentValidation;
using Saldoa.Application.Auth.PasswordReset;

namespace Saldoa.Application.Auth.ResetPassword
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("O id do usuário é obrigatório");

            RuleFor(x => x.EncodedToken)
                .NotEmpty()
                .WithMessage("O token é obrigatório");

            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("A senha é obrigatória")
                .MinimumLength(8).WithMessage("A senha precisa ter no mínimo 8 caracteres")
                .Matches("[0-9]").WithMessage("A senha precisa conter ao menos um número")
                .Matches("[a-z]").WithMessage("A senha precisa conter ao menos uma letra minúscula")
                .Matches("[A-Z]").WithMessage("A senha precisa conter ao menos uma letra maiúscula")
                .Matches("[^a-zA-Z0-9]").WithMessage("A senha precisa conter ao menos um caractere especial");
        }
    }
}
