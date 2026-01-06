using System.Reflection;
using MeuBolso.API.Identity;
using MeuBolso.Domain.Entities;
using MeuBolso.Domain.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeuBolso.API.Persistence
{
    public class MeuBolsoDbContext : IdentityDbContext<ApplicationUser>
    {
        public MeuBolsoDbContext(DbContextOptions<MeuBolsoDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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