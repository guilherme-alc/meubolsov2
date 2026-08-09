using Saldoa.Application.Auth.Common;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Email;
using Saldoa.Application.Email.Abstractions;
using Saldoa.Application.Identity.Abstractions;

namespace Saldoa.Application.Auth.ResendConfirmEmail
{
    public class ResendConfirmEmailUseCase
    {
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;

        public ResendConfirmEmailUseCase(IIdentityService identityService, IEmailService emailService)
        {
            _identityService = identityService;
            _emailService = emailService;
        }

        public async Task<Result> ExecuteAsync(ResendConfirmEmailRequest request, CancellationToken ct)
        {
            if(string.IsNullOrWhiteSpace(request.Email))
                return Result.Failure(AuthErrors.Invalid);

            var validateResult = await _identityService.PrepareEmailConfirmationResendAsync(request.Email, ct);
            
            if (!validateResult.IsSuccess)
                return Result.Failure(validateResult.Error!);

            if (!validateResult.Value!.ShouldSendEmail)
                return Result.Success();

            var confirmToken = validateResult.Value.Token!;

            var body = $"""
            <h1>Confirme seu e-mail</h1>

            <p>
                Seu token é: {confirmToken}
            </p>
            """;

            EmailMessage emailMessage = new(validateResult.Value.Email!, "Bem-vindo ao Saldoa! - Confirme seu e-mail", body);

            await _emailService.SendAsync(emailMessage, ct);

            var resultUpdate = await _identityService.UpdateLastConfirmationEmailSentAtAsync(validateResult.Value.UserId!, ct);

            if (!resultUpdate.IsSuccess)
                return Result.Failure(resultUpdate.Error!);

            return Result.Success();
        }
    }
}
