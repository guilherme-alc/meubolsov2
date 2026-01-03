using MeuBolso.API.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeuBolso.API.Persistence.Configurations;

public class ApplicationUserConfiguration :  IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("users", "auth");
        
        builder.Property(u => u.FullName)
            .HasColumnName("full_name")
            .IsRequired(false)
            .HasMaxLength(255);
        
        builder.Property(u => u.CustomerId)
            .HasColumnName("customer_id")
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
    }
}