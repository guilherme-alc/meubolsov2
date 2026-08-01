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

    public async Task<Result<CreateUserResult>> CreateUserAsync(string email, string password, string? fullName, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            IsActive =  true,
            IsPremium = false,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new Exception(string.Join(
                ", ", result.Errors.Select(e => e.Description)));

        var rawConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawConfirmationToken));

        return Result<CreateUserResult>.Success(new CreateUserResult(confirmationToken, email, user.Id));
    }

    public async Task<Result<string>> SignInAsync(string email, string password, CancellationToken ct = default)
    {
        var normalized = _userManager.NormalizeEmail(email);
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalized, ct);
        
        if (user is null || !user.IsActive)
            return Result<string>.Failure(AuthErrors.InvalidCredentials);

        ct.ThrowIfCancellationRequested();

        var passwordIsValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordIsValid)
            return Result<string>.Failure(AuthErrors.InvalidCredentials);

        var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        if(!emailConfirmed)
            return Result<string>.Failure(AuthErrors.EmailNotConfirmed);

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

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
            return Result.Failure(AuthErrors.InvalidConfirmToken);

        return Result.Success();
    }
}
