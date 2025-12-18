using MeuBolso.Domain.Entities;
using MeuBolso.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeuBolso.Infrastructure.Mappings
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("transactions", "app");

            builder.HasKey(t => t.Id);

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
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("now()")
                .IsRequired();

            builder.Property(t => t.PaymentOrReceivedAt)
                .HasColumnName("payment_or_received_at")
                .HasColumnType("timestamp without time zone")
                .IsRequired(false);
                
            builder.Property(t => t.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}