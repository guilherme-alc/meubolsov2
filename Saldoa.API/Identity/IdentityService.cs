using Saldoa.Application.Identity.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Saldoa.API.Identity;

internal sealed class IdentityService : IIdentityService
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

    public async Task<string> CreateUserAsync(string email, string password, string? fullName, CancellationToken ct = default)
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

        return user.Id;
    }

    public async Task<string?> SignInAsync(string email, string password, CancellationToken ct = default)
    {
        var normalized = _userManager.NormalizeEmail(email);
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalized, ct);
        
        if (user is null || !user.IsActive)
            return null;
        
        ct.ThrowIfCancellationRequested();
        var ok = await _userManager.CheckPasswordAsync(user, password);
        
        if(!ok)
            return null;

        user.LastLoginAt = DateTime.UtcNow;
        
        ct.ThrowIfCancellationRequested();
        var updateLastLoginResult = await _userManager.UpdateAsync(user);
        if (!updateLastLoginResult.Succeeded)
            throw new Exception(string.Join(", ", updateLastLoginResult.Errors.Select(e => e.Description)));
        
        return user.Id;
    }
    
    public async Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userManager.Users
            .SingleOrDefaultAsync(u => u.Id == userId, ct);
        
        return user?.Email;
    }
}
