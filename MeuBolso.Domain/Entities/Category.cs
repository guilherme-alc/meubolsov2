namespace MeuBolso.Domain.Entities
{
    public class Category
    {
        private Category() { }
        public Category(string userId, string name, string? description, string? color)
        {
            UserId = userId;
            SetName(name);
            SetDescription(description);
            SetColor(color);
        }

        public long Id { get; private set; }
        public string Name { get; private set; }
        public string NormalizedName { get; private set; }
        public string? Description { get; private set; }
        public string? Color { get; private set; }
        public string UserId { get; private set; } = string.Empty;
        
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome da categoria é obrigatório.", nameof(name));

            Name = name.Trim();
            NormalizedName = Name.ToUpperInvariant();
        }

        public void SetDescription(string? description) => Description = description?.Trim();
        public void SetColor(string? color) => Color = color?.Trim();
        
    }
}