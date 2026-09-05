using Saldoa.Application.Auth.Common;
using Saldoa.Application.Auth.ConfirmEmail;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Email;
using Saldoa.Application.Identity.Abstractions;

namespace Saldoa.Application.Auth.Register;

public class RegisterUseCase
{
    private readonly IIdentityService _identityService;

    private readonly IBackgroundJobService _backgroundJobService;

    public RegisterUseCase(IIdentityService identityService, IBackgroundJobService backgroundJobService)
    {
        _identityService = identityService;
        _backgroundJobService = backgroundJobService;
    }

    public async Task<Result> ExecuteAsync(RegisterRequest request, CancellationToken ct)
    {
        if (await _identityService.UserExistsAsync(request.Email, ct))
        {
            var error = AuthErrors.AlreadyExists;
            return Result.Failure(error);
        }
        
        var result = await _identityService.CreateUserAsync(
            request.Email, 
            request.Password, 
            request.FirstName,
            request.LastName,
            ct);
        
        if(!result.IsSuccess)
        {
            return Result.Failure(result.Error!);
        }

        var body = $"""
            <h1>Confirme seu e-mail</h1>
            <p>
                Seu Id é: {result.Value!.UserId}
            </p>
            <p>
                Seu token é: {result.Value.ConfirmationToken}
            </p>
            """;

        EmailMessage emailMessage = new(result.Value.Email, "Bem-vindo ao Saldoa! - Confirme seu e-mail", body);

        _backgroundJobService.Enqueue<SendEmailConfirmationJob>(x => x.ExecuteAsync(emailMessage, result.Value.UserId, CancellationToken.None));

        return Result.Success();
    }
}