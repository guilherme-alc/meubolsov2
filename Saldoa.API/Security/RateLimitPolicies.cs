namespace Saldoa.API.Security;

public static class RateLimitPolicies
{
    public const string Login = "auth-login";
    public const string Register = "auth-register";
    public const string PasswordRecovery = "auth-password-recovery";
    public const string EmailConfirmation = "auth-email-confirmation";
    public const string Refresh = "auth-refresh";
}
