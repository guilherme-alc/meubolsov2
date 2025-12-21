using MeuBolso.Domain.Enums;

namespace MeuBolso.Domain.Entities
{
    public class Transaction
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ETransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaymentOrReceivedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public long CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
    }
}