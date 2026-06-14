using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Saldoa.Infrastructure.Identity;
using Saldoa.Domain.Auth;
using Saldoa.Domain.Entities;

namespace Saldoa.Infrastructure.Persistence
{
    public class SaldoaDbContext : IdentityDbContext<ApplicationUser>
    {
        public SaldoaDbContext(DbContextOptions<SaldoaDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<CategoryBudget> CategoryBudgets { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.HasDefaultSchema("app");
            
            builder.Entity<IdentityRole>().ToTable("roles", "auth");
            builder.Entity<IdentityUserRole<string>>().ToTable("user_roles", "auth");
            builder.Entity<IdentityUserClaim<string>>().ToTable("user_claims", "auth");
            builder.Entity<IdentityUserLogin<string>>().ToTable("user_logins", "auth");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims", "auth");
            builder.Entity<IdentityUserToken<string>>().ToTable("user_tokens", "auth");

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}