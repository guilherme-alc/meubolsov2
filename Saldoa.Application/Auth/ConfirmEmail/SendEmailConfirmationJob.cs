using Saldoa.Application.Email;
using Saldoa.Application.Email.Abstractions;
using Saldoa.Application.Identity.Abstractions;

namespace Saldoa.Application.Auth.ConfirmEmail
{
    public class SendEmailConfirmationJob
    {
        private readonly IEmailService _emailService;
        private readonly IIdentityService _identityService;

        public SendEmailConfirmationJob(
            IEmailService emailService,
            IIdentityService identityService)
        {
            _emailService = emailService;
            _identityService = identityService;
        }

        public async Task ExecuteAsync(EmailMessage emailMessage, string userId, CancellationToken ct = default)
        {
            await _emailService.SendAsync(emailMessage, ct);

            var updateResult = await _identityService.UpdateLastConfirmationEmailSentAtAsync(userId, ct);

            if (!updateResult.IsSuccess)
            {
                var error = updateResult.Error!;

                throw new InvalidOperationException(
                    $"Falha ao atualizar a data de envio do e-mail de confirmação. Code: {error.Code}. Message: {error.Message}");
            }
        }
    }
}
