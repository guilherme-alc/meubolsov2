using Saldoa.Domain.Exceptions;

namespace Saldoa.Domain.Entities
{
    public class Category
    {
        private Category() { }
        public Category(string userId, string name, string? description, string? color)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new DomainException("Usuário inválido.");
            SetName(name);
            SetDescription(description);
            SetColor(color);
            UserId = userId;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public long Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string NormalizedName { get; private set; } = null!;
        public string? Description { get; private set; }
        public string? Color { get; private set; }
        public string UserId { get; private set; } = null!;
        public DateTimeOffset CreatedAt { get; private set; }
        
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Nome da categoria é obrigatório.");

            Name = name.Trim();
            NormalizedName = Name.ToUpperInvariant();
        }

        public void SetDescription(string? description)
        {
            var d = description?.Trim();
            Description = string.IsNullOrWhiteSpace(d) ? null : d;
        }
        
        public void SetColor(string? color)
        {
            var c = color?.Trim();
            Color = string.IsNullOrWhiteSpace(c) ? null : c;
        }
    }
}