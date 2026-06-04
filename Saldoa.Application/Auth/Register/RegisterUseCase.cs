using Saldoa.Application.Auth.Common;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Identity.Abstractions;

namespace Saldoa.Application.Auth.Register;

public class RegisterUseCase
{
    private readonly IIdentityService _identityService;

    public RegisterUseCase(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<Result<string>> ExecuteAsync(RegisterRequest request, CancellationToken ct)
    {
        if (await _identityService.UserExistsAsync(request.Email, ct))
        {
            var error = AuthErrors.AlreadyExists;
            return Result<string>.Failure(error);
        }
        
        var userId = await _identityService.CreateUserAsync(
            request.Email, 
            request.Password, 
            request.FullName,
            ct);

        return Result<string>.Success(userId);
    }
}