namespace Saldoa.Application.Auth.ConfirmEmail
{
    public sealed record ConfirmEmailRequest(string UserId, string EncodedToken);
}
