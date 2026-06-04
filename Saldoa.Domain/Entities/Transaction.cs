using Saldoa.Domain.Enums;
using Saldoa.Domain.Exceptions;
using Saldoa.Domain.ValueObjects;

namespace Saldoa.Domain.Entities
{
    public class Transaction
    {
        private Transaction() { }
        public Transaction(
            string userId,
            string title,
            string? description,
            TransactionType type,
            decimal totalAmount,
            DateOnly paidOrReceivedAt,
            long categoryId,
            InstallmentInfo installmentInfo)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new DomainException("Usuário inválido.");
            UserId = userId;
            InitTitle(title);
            SetDescription(description);
            Type = type;
            SetAmount(totalAmount);
            PaidOrReceivedAt = paidOrReceivedAt;
            CreatedAt = DateTimeOffset.UtcNow;
            CategoryId = categoryId;
            InstallmentInfo = installmentInfo ?? throw new DomainException(nameof(installmentInfo));   
        }

        public long Id { get; private set; }
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }
        public TransactionType Type { get; private set; }
        public decimal Amount { get; private set; }
        public DateOnly PaidOrReceivedAt { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public long CategoryId { get; private set; }
        public Category Category { get; private set; } = null!;
        public InstallmentInfo InstallmentInfo { get; private set; } = null!;
        public string UserId { get; private set; } = null!;

        public void InitTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Título é obrigatório.");

            Title = title.Trim();
        }
        public void SetTitle(string? title)
        {
            if (title is null)
                return;
            var t = title.Trim();
            if (string.IsNullOrWhiteSpace(t))
                return;
            Title = t;
        }

        public void SetDescription(string? description)
        {
            if (description == null)
                return;

            var d = description?.Trim();
            Description = string.IsNullOrWhiteSpace(d) ? null : d;
        }

        public void SetAmount(decimal? amount)
        {
            if (amount.HasValue)
                Amount = Math.Abs(amount.Value);
        }

        public void SetPaidOrReceivedAt(DateOnly? paidOrReceivedAt)
        {
            if (paidOrReceivedAt.HasValue)
                PaidOrReceivedAt = paidOrReceivedAt.Value;
        }

        public void SetCategoryId(long? categoryId)
        {
            if (!categoryId.HasValue)
                return;

            if (categoryId.Value <= 0)
                throw new DomainException("Categoria inválida.");
            CategoryId = categoryId.Value;
        }
    }
}