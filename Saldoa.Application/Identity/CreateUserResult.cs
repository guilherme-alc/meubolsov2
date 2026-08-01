namespace Saldoa.Application.Identity
{
    public sealed record CreateUserResult(string ConfirmationToken, string Email, string UserId);
}
