namespace Saldoa.API.Security;

internal static class RateLimitPolicies
{
    internal const string Login = "auth-login";
    internal const string Register = "auth-register";
    internal const string PasswordResetRequest = "auth-password-reset-request";
    internal const string PasswordResetConfirm = "auth-password-reset-confirm";
    internal const string EmailConfirmation = "auth-email-confirmation";
    internal const string Refresh = "auth-refresh";
}
