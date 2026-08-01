using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Saldoa.Application.Email;
using Saldoa.Application.Email.Abstractions;

namespace Saldoa.Infrastructure.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailOptions _options;

        public SmtpEmailService(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();
            var address = new MailboxAddress(_options.FromName, _options.FromEmail);

            message.From.Add(address);
            message.To.Add(MailboxAddress.Parse(emailMessage.Recipient));

            message.Subject = emailMessage.Subject;
            message.Body = new BodyBuilder
            {
                HtmlBody = emailMessage.HtmlBody
            }.ToMessageBody();

            using SmtpClient smtpClient = new();

            var socketOptions = _options.UseSsl ? SecureSocketOptions.Auto : SecureSocketOptions.None;

            await smtpClient.ConnectAsync(
                _options.Host,
                _options.Port,
                socketOptions,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(_options.Username) && !string.IsNullOrWhiteSpace(_options.Password))
            {
                await smtpClient.AuthenticateAsync(
                    _options.Username,
                    _options.Password,
                    cancellationToken);
            }

            await smtpClient.SendAsync(
                message,
                cancellationToken);

            await smtpClient.DisconnectAsync(
                quit: true,
                cancellationToken);
        }
    }
}
