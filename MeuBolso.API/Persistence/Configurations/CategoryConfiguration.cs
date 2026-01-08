using MeuBolso.API.Identity;
using MeuBolso.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeuBolso.API.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories", "app");

            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Id)
                .HasColumnName("id");

            builder.Property(c => c.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(x => x.NormalizedName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasColumnName("description")
                .IsRequired(false)
                .HasMaxLength(255);

            builder.Property(c => c.Color)
                .HasColumnName("color")
                .IsRequired(false)
                .HasMaxLength(12);

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(c => c.UserId)
               .HasConstraintName("fk_categories_user")
               .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasIndex(c => new { c.UserId, c.NormalizedName })
                .IsUnique()
                .HasDatabaseName("ux_categories_user_normalized_name");
        }
    }
}