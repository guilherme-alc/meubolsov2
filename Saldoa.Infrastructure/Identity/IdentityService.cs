using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Saldoa.Application.Auth.Common;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Identity;
using Saldoa.Application.Identity.Abstractions;
using System.Text;

namespace Saldoa.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    public Task<bool> UserExistsAsync(string email, CancellationToken ct = default)
    {
        var normalized = _userManager.NormalizeEmail(email);
        return _userManager.Users.AnyAsync(u => u.NormalizedEmail == normalized, ct);
    }

    public async Task<Result<CreateUserResult>> CreateUserAsync(string email, string password, string firstName, string? lastName, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            IsActive =  true,
            IsPremium = false,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new Exception(string.Join(
                ", ", result.Errors.Select(e => e.Description)));

        var confirmationToken = await GenerateEmailConfirmationTokenAsync(user.Id, ct);

        if (!confirmationToken.IsSuccess || string.IsNullOrWhiteSpace(confirmationToken.Value))
            return Result<CreateUserResult>.Failure(AuthErrors.Unexpected);

        return Result<CreateUserResult>.Success(new CreateUserResult(confirmationToken.Value!, email, user.Id));
    }

    private async Task<Result<string>> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result<string>.Failure(AuthErrors.UserNotFound);

        var rawConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawConfirmationToken));

        return Result<string>.Success(confirmationToken);
    }

    public async Task<Result<string>> SignInAsync(string email, string password, CancellationToken ct = default)
    {
        var normalized = _userManager.NormalizeEmail(email);

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalized, ct);
        
        if (user is null || !user.IsActive)
            return Result<string>.Failure(AuthErrors.InvalidCredentials);

        if (await _userManager.IsLockedOutAsync(user))
            return Result<string>.Failure(AuthErrors.Forbidden);

        if (!await _userManager.IsEmailConfirmedAsync(user))
            return Result<string>.Failure(AuthErrors.EmailNotConfirmed);

        ct.ThrowIfCancellationRequested();

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user);

            if (await _userManager.IsLockedOutAsync(user))
                return Result<string>.Failure(AuthErrors.Forbidden);

            return Result<string>.Failure(AuthErrors.InvalidCredentials);
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        user.LastLoginAt = DateTime.UtcNow;
        
        ct.ThrowIfCancellationRequested();
        var updateLastLoginResult = await _userManager.UpdateAsync(user);

        if (!updateLastLoginResult.Succeeded)
            throw new Exception(string.Join(", ", updateLastLoginResult.Errors.Select(e => e.Description)));

        return Result<string>.Success(user.Id);
    }
    
    public async Task<Result<string?>> GetEmailByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userManager.Users
            .SingleOrDefaultAsync(u => u.Id == userId, ct);
        
        if(user is null)
            return Result<string?>.Failure(AuthErrors.UserNotFound);

        return Result<string?>.Success(user.Email);
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string encodedToken, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure(AuthErrors.UserNotFound);

        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken.Trim()));
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException)
        {
            return Result.Failure(AuthErrors.InvalidConfirmToken);
        }

        ct.ThrowIfCancellationRequested();

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
            return Result.Failure(AuthErrors.InvalidConfirmToken);

        return Result.Success();
    }

    public async Task<Result> UpdateLastConfirmationEmailSentAtAsync(string userId, CancellationToken ct)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
            return Result.Failure(AuthErrors.UserNotFound);

        user.LastConfirmationEmailSentAt = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Result.Success();
    }

    public async Task<Result<ConfirmationTokenResult>> PrepareEmailConfirmationResendAsync(string email, CancellationToken ct)
    {
        var normalizedEmail = _userManager.NormalizeEmail(email);

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, ct);

        if (user is null)
            return Result<ConfirmationTokenResult>.Success(new(false, null, null, null));

        if (user.EmailConfirmed)
            return Result<ConfirmationTokenResult>.Success(new(false, null, null, null));

        if (user.LastConfirmationEmailSentAt.HasValue && user.LastConfirmationEmailSentAt.Value.AddMinutes(5) > DateTime.UtcNow)
            return Result<ConfirmationTokenResult>.Success(new(false, null, null, null));

        var confirmationToken = await GenerateEmailConfirmationTokenAsync(user.Id, ct);

        if (!confirmationToken.IsSuccess || string.IsNullOrWhiteSpace(confirmationToken.Value))
            return Result<ConfirmationTokenResult>.Failure(AuthErrors.Unexpected);

        return Result<ConfirmationTokenResult>.Success(
            new ConfirmationTokenResult(
                ShouldSendEmail: true, 
                Email: user.Email, 
                UserId: user.Id, 
                Token: confirmationToken.Value)
        );
    }

    public async Task<Result<ResetPasswordTokenResult>> GenerateResetPasswordTokenAsync(string email, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var normalizedEmail = _userManager.NormalizeEmail(email);
        var user = await _userManager.FindByEmailAsync(normalizedEmail);

        if (user is null)
            return Result<ResetPasswordTokenResult>.Success(new(false, null, null, null));

        if (user.LastPasswordResetEmailSentAt.HasValue &&
            user.LastPasswordResetEmailSentAt.Value.AddMinutes(3) > DateTime.UtcNow)
            return Result<ResetPasswordTokenResult>.Success(new(false, null, null, null));

        var rawConfirmationToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var passwordResetToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawConfirmationToken));

        if (string.IsNullOrWhiteSpace(passwordResetToken))
            return Result<ResetPasswordTokenResult>.Failure(AuthErrors.Unexpected);

        return Result<ResetPasswordTokenResult>.Success(new ResetPasswordTokenResult(
            ShouldSendEmail: true,
            Email: user.Email,
            UserId: user.Id,
            Token: passwordResetToken)
        );
    }

    public async Task<Result> UpdateLastPasswordResetEmailSentAtAsync(string userId, CancellationToken ct)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
            return Result.Failure(AuthErrors.UserNotFound);

        user.LastPasswordResetEmailSentAt = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string userId, string encodedToken, string newPassword, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure(AuthErrors.InvalidResetToken);

        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken.Trim()));
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException)
        {
            return Result.Failure(AuthErrors.InvalidResetToken);
        }

        ct.ThrowIfCancellationRequested();

        var passwordResetResult = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);

        if (!passwordResetResult.Succeeded)
        {
            var passwordErrors = passwordResetResult.Errors
                .Where(e => e.Code.StartsWith("Password", StringComparison.OrdinalIgnoreCase));

            if (passwordErrors.Any())
                return Result.Failure(AuthErrors.InvalidPassword);

            return Result.Failure(AuthErrors.InvalidResetToken);
        }

        user.LastPasswordResetAt = DateTime.UtcNow;

        var updateUserResult = await _userManager.UpdateAsync(user);

        if (!updateUserResult.Succeeded)
            throw new Exception(string.Join(", ", updateUserResult.Errors.Select(e => e.Description)));

        return Result.Success();
    }
}
