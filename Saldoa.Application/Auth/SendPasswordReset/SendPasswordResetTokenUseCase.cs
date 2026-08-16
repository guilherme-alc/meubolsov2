using Saldoa.Application.Auth.Common;
using Saldoa.Application.Auth.ResendConfirmEmail;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Email;
using Saldoa.Application.Email.Abstractions;
using Saldoa.Application.Identity.Abstractions;

namespace Saldoa.Application.Auth.SendPasswordReset
{
    public class SendPasswordResetTokenUseCase
    {
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;
        public SendPasswordResetTokenUseCase(IIdentityService identityService, IEmailService emailService)
        {
            _identityService = identityService;
            _emailService = emailService;
        }

        public async Task<Result> ExecuteAsync(SendPasswordResetTokenRequest request, CancellationToken ct = default)
        {
            if(string.IsNullOrWhiteSpace(request.Email))
                return Result.Failure(AuthErrors.Invalid);
            
             var generateTokenResult = await _identityService.GenerateResetPasswordTokenAsync(request.Email, ct);

            if(!generateTokenResult.IsSuccess)
                return Result.Failure(generateTokenResult.Error!);

            if (!generateTokenResult.Value!.ShouldSendEmail)
                return Result.Success();


            var resetToken = generateTokenResult.Value.Token!;

            var body = $"""
            <h1>Redefinir senha</h1>
            <p>
                Seu Id é: {generateTokenResult.Value.UserId}
            </p>
            <p>
                Seu código para redefinir a senha é: {resetToken}
            </p>
            """;

            EmailMessage emailMessage = new(generateTokenResult.Value.Email!, "Saldoa - Redefinir senha", body);

            await _emailService.SendAsync(emailMessage, ct);

            var resultUpdate = await _identityService.UpdateLastPasswordResetEmailSentAtAsync(generateTokenResult.Value.UserId!, ct);

            if (!resultUpdate.IsSuccess)
                return Result.Failure(resultUpdate.Error!);

            return Result.Success();
        }
    }
}
