using Saldoa.Application.Auth.Common;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Email;
using Saldoa.Application.Email.Abstractions;
using Saldoa.Application.Identity.Abstractions;

namespace Saldoa.Application.Auth.Register;

public class RegisterUseCase
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;

    public RegisterUseCase(IIdentityService identityService, IEmailService emailService)
    {
        _identityService = identityService;
        _emailService = emailService;
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
            request.FullName,
            ct);
        
        if(!result.IsSuccess)
        {
            return Result.Failure(result.Error!);
        }

        var body = $"""
            <h1>Confirme seu e-mail</h1>

            <p>
                Seu token é: {result.Value!.ConfirmationToken}
            </p>
            <p>
                Usuário: {result.Value.UserId}
            </p>
            """;

        EmailMessage emailMessage = new(result.Value.Email, "Bem-vindo ao Saldoa! - Confirme seu e-mail", body);

        await _emailService.SendAsync(emailMessage, ct);

        return Result.Success();
    }
}