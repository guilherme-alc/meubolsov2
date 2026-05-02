using Saldoa.Application.Auth.Abstractions;
using Saldoa.Application.Auth.Common;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Identity.Abstractions;

namespace Saldoa.Application.Auth.Register;

public class RegisterUseCase
{
    private readonly IIdentityService _identityService;
    private readonly IJwtProvider _jwtProvider;

    public RegisterUseCase(IIdentityService identityService, IJwtProvider jwtProvider)
    {
        _identityService = identityService;
        _jwtProvider = jwtProvider;
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