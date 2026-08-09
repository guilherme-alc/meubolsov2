namespace Saldoa.Application.Identity
{
    public sealed record ConfirmationTokenResult(
        bool ShouldSendEmail,
        string? Email,
        string? UserId,
        string? Token);
}
