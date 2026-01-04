using MeuBolso.Application.Identity.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace MeuBolso.API.Identity;

internal sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<bool> UserExistsAsync(string email)
        => await _userManager.FindByEmailAsync(email) is not null;

    public async Task<string> CreateUserAsync(string email, string password, string? fullName)
    {
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

    public async Task<string?> ValidateCredentialsAndGetUserIdAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return null;
        
        if (!user.IsActive) 
            return null;
        
        var ok = await _userManager
            .CheckPasswordAsync(user, password);
        
        if(!ok)
            return null;

        user.LastLoginAt = DateTime.UtcNow;
        var updateLastLoginResult = await _userManager.UpdateAsync(user);
        if (!updateLastLoginResult.Succeeded)
            throw new Exception(string.Join(", ", updateLastLoginResult.Errors.Select(e => e.Description)));
        
        return user.Id;
    }
}