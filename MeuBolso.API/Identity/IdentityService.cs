using MeuBolso.Application.Identity.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace MeuBolso.API.Identity;

internal sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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

    public async Task<bool> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return false;

        var result = await _signInManager
            .CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

        return result.Succeeded;
    }
}