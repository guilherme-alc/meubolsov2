namespace Saldoa.Infrastructure.Email
{
    public sealed class EmailOptions
    {
        public const string SectionName = "Email";

        public string Host { get; init; } = string.Empty;
        public int Port { get; init; }
        public bool UseSsl { get; init; }

        public string? Username { get; init; }
        public string? Password { get; init; }

        public string FromEmail { get; init; } = string.Empty;
        public string FromName { get; init; } = string.Empty;
    }
}
