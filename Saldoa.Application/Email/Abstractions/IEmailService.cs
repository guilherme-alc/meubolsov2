namespace Saldoa.Application.Email.Abstractions
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
    }
}
