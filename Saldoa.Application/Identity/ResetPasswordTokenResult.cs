namespace Saldoa.Application.Identity
{
    public sealed record ResetPasswordTokenResult(
        bool ShouldSendEmail,
        string? Email,
        string? UserId,
        string? Token);
}
