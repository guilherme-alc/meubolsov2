using Saldoa.Application.Auth.Common;
using Saldoa.Application.Auth.PasswordReset;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Identity.Abstractions;

namespace Saldoa.Application.Auth.ResetPassword
{
    public class ResetPasswordUseCase
    {
        private readonly IIdentityService _identityService;
        public ResetPasswordUseCase(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result> ExecuteAsync(ResetPasswordRequest request, CancellationToken ct = default)
        {
            if (request.UserId is null || request.EncodedToken is null)
                return Result.Failure(AuthErrors.Invalid);

            var result = await _identityService.ResetPasswordAsync(request.UserId, request.EncodedToken, request.NewPassword, ct);

            if (!result.IsSuccess)
                return Result.Failure(result.Error!);

            return Result.Success();
        }
    }
}
