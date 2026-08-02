using Microsoft.AspNetCore.Identity;

namespace Saldoa.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsPremium { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastConfirmationEmailSentAt { get; set; }
    }
}