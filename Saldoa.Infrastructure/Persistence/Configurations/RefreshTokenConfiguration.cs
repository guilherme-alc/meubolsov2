using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saldoa.Domain.Auth;

namespace Saldoa.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens", "auth");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id)
            .HasColumnName("id");

        builder.Property(rt => rt.TokenHash)
            .HasColumnName("token_hash")
            .IsRequired();

        builder.Property(rt => rt.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(rt => rt.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();
        
        builder.Property(x => x.RevokedAt)
            .HasColumnName("revoked_at");

        builder.Property(x => x.ReplacedByTokenHash)
            .HasColumnName("replaced_by_token_hash");
        
        builder.Ignore(x => x.IsExpired);
        builder.Ignore(x => x.IsRevoked);
        
        builder.HasIndex(rt => rt.TokenHash)
            .IsUnique();
        builder.HasIndex(rt => rt.UserId);
    }
}