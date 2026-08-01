using Saldoa.Application.Auth.Common;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Identity.Abstractions;

namespace Saldoa.Application.Auth.ConfirmEmail
{
    public class ConfirmEmailUseCase
    {
        private readonly IIdentityService _identityService;
        public ConfirmEmailUseCase(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result> ExecuteAsync(ConfirmEmailRequest request, CancellationToken ct)
        {
            if (request.UserId is null || request.EncodedToken is null)
                return Result.Failure(AuthErrors.Invalid);

            var result = await _identityService.ConfirmEmailAsync(request.UserId, request.EncodedToken, ct);
            if (!result.IsSuccess)
                return Result.Failure(result.Error!);

            return Result.Success();
        }
    }
}
