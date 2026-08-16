using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Identity.Abstractions;

public interface IIdentityService
{
    Task<bool> UserExistsAsync(string email, CancellationToken ct);
    Task<Result<CreateUserResult>> CreateUserAsync(string email, string password, string firstName, string? lastName, CancellationToken ct);
    Task<Result<string>> SignInAsync(string email, string password, CancellationToken ct);
    Task<Result<string?>> GetEmailByUserIdAsync(string userId, CancellationToken ct = default);
    Task<Result> ConfirmEmailAsync(string userId, string encodedToken, CancellationToken ct);
    Task<Result> UpdateLastConfirmationEmailSentAtAsync(string userId, CancellationToken ct);
    Task<Result<ConfirmationTokenResult>> PrepareEmailConfirmationResendAsync(string email, CancellationToken ct);
    Task<Result<ResetPasswordTokenResult>> GenerateResetPasswordTokenAsync(string email, CancellationToken ct);
}
