namespace Saldoa.Domain.Entities
{

    public class CategoryBudget
    {
        private CategoryBudget() { }

        public CategoryBudget(long categoryId, DateOnly periodStart, DateOnly periodEnd, decimal limitAmount, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Usuário inválido.", nameof(userId));
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
                throw new ArgumentOutOfRangeException(nameof(categoryId), "Categoria inválida.");
        
            CategoryId = categoryId;
        }
    
        public void SetLimitAmount(decimal limitAmount)
        {
            if (limitAmount <= 0)
                throw new ArgumentOutOfRangeException(nameof(limitAmount), "O limite deve ser maior que zero.");

            LimitAmount = limitAmount;
        }

        public void SetPeriod(DateOnly start, DateOnly end)
        {
            if (start > end)
                throw new ArgumentException("O início do período não pode ser maior que o fim.");

            PeriodStart = start;
            PeriodEnd = end;
        }
    }
}