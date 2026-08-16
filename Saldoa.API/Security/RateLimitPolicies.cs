namespace Saldoa.API.Security;

public static class RateLimitPolicies
{
    public const string Login = "auth-login";
    public const string Register = "auth-register";
    public const string PasswordResetRequest = "auth-password-reset-request";
    public const string PasswordResetConfirm = "auth-password-reset-confirm";
    public const string EmailConfirmation = "auth-email-confirmation";
    public const string Refresh = "auth-refresh";
}
