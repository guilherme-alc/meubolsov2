namespace Saldoa.Application.Auth.PasswordReset
{
    public sealed record ResetPasswordRequest(string UserId, string EncodedToken, string NewPassword);
}
