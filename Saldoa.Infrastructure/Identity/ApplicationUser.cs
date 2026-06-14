using Microsoft.AspNetCore.Identity;

namespace Saldoa.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsPremium { get; set; } = false;
        public string? CustomerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
    }
}