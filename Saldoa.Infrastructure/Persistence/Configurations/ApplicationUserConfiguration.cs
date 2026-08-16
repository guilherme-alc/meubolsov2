using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saldoa.Infrastructure.Identity;

namespace Saldoa.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration :  IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("users", "auth");
        
        builder.Property(u => u.FirstName)
            .HasColumnName("first_name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .HasColumnName("last_name")
            .IsRequired(false)
            .HasMaxLength(100);
        
        builder.Property(u => u.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(u => u.IsPremium)
            .HasColumnName("is_premium")
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(u => u.LastLoginAt)
            .HasColumnName("last_login_at");

        builder.Property(u => u.LastConfirmationEmailSentAt)
            .HasColumnName("last_confirmation_email_sent_at");

        builder.Property(u => u.LastPasswordResetEmailSentAt)
            .HasColumnName("last_password_reset_email_sent_at");

        builder.Property(u => u.LastPasswordResetAt)
            .HasColumnName("last_password_reset_at");

    }
}