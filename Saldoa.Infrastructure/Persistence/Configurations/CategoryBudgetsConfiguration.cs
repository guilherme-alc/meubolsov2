using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saldoa.Infrastructure.Identity;
using Saldoa.Domain.Entities;

namespace Saldoa.Infrastructure.Persistence.Configurations;

public class CategoryBudgetsConfiguration 
    : IEntityTypeConfiguration<CategoryBudget>
{
    public void Configure(EntityTypeBuilder<CategoryBudget> builder)
    {
        builder.ToTable("category_budgets", "app");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id");

        builder.Property(c => c.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(c => c.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();

        builder.Property(c => c.PeriodStart)
            .HasColumnName("period_start")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.PeriodEnd)
            .HasColumnName("period_end")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        
        builder.Property(c => c.LimitAmount)
            .HasColumnName("limit_amount")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.HasOne(c => c.Category)
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .HasConstraintName("fk_category_budgets_category")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .HasConstraintName("fk_category_budgets_user")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new 
        { 
            c.UserId, 
            c.CategoryId, 
            c.PeriodStart, 
            c.PeriodEnd 
        })
        .IsUnique();
    }
}
