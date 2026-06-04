using Saldoa.Domain.Exceptions;

namespace Saldoa.Domain.Entities
{

    public class CategoryBudget
    {
        private CategoryBudget() { }

        public CategoryBudget(long categoryId, DateOnly periodStart, DateOnly periodEnd, decimal limitAmount, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new DomainException("Usuário inválido.");
            SetCategoryId(categoryId);
            SetPeriod(periodStart, periodEnd);
            SetLimitAmount(limitAmount);
            UserId = userId;
            CreatedAt = DateTimeOffset.UtcNow;
        }
        public long Id { get; private set; }
        public long CategoryId { get; private set; }
        public string UserId { get; private set; } = null!;
        public DateOnly PeriodStart { get; private set; }
        public DateOnly PeriodEnd { get; private set; }
        public decimal LimitAmount { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public Category Category { get; private set; } = default!;
    
        public void SetCategoryId(long categoryId)
        {
            if (categoryId <= 0) 
                throw new DomainException("Categoria inválida.");
        
            CategoryId = categoryId;
        }
    
        public void SetLimitAmount(decimal limitAmount)
        {
            if (limitAmount < 0)
                throw new DomainException("O limite não pode ser negativo.");

            LimitAmount = limitAmount;
        }

        public void SetPeriod(DateOnly start, DateOnly end)
        {
            if (start > end)
                throw new DomainException("O início do período não pode ser maior que o fim.");

            PeriodStart = start;
            PeriodEnd = end;
        }
    }
}