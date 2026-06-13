using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saldoa.API.Identity;
using Saldoa.Domain.Entities;

namespace Saldoa.API.Infrastructure.Persistence.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("transactions", "app");

            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.Id)
                .HasColumnName("id");

            builder.Property(t => t.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Description)
                .HasColumnName("description")
                .IsRequired(false)
                .HasMaxLength(255);

            builder.Property(t => t.Type)
                .HasColumnName("type")
                .HasConversion<short>()
                .IsRequired()
                .HasComment("1 = Deposit, 2 = Withdraw");

            builder.Property(t => t.Amount)
                .HasColumnName("amount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(t => t.PaidOrReceivedAt)
                .HasColumnName("paid_or_received_at")
                .HasColumnType("date")
                .IsRequired();
                
            builder.OwnsOne(t => t.InstallmentInfo, i =>
            {
                i.Property(ii => ii.TotalInstallments)
                    .HasColumnName("installment_total")
                    .IsRequired();

                i.Property(ii => ii.InstallmentNumber)
                    .HasColumnName("installment_number")
                    .IsRequired();

                i.Property(ii => ii.InstallmentGroupId)
                    .HasColumnName("installment_group_id");

                i.HasIndex(ii => ii.InstallmentGroupId)
                .HasFilter("installment_group_id IS NOT NULL");
            });

            builder.Property(t => t.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(t => t.UserId)
               .HasConstraintName("fk_transactions_user")
               .OnDelete(DeleteBehavior.Cascade);
            
            builder.Property(t => t.CategoryId)
                .HasColumnName("category_id")
                .IsRequired();
            
            builder.HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .HasConstraintName("fk_transactions_category")
                .OnDelete(DeleteBehavior.Restrict);    

            builder.HasIndex(t => new
            {
                t.UserId,
                t.PaidOrReceivedAt
            });
        }
    }
}