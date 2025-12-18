using MeuBolso.Domain.Entities;
using MeuBolso.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MeuBolso.Infrastructure.Data.DbContext
{
    public class MeuBolsoDbContext : IdentityDbContext<ApplicationUser>
    {
        public MeuBolsoDbContext(DbContextOptions<MeuBolsoDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.HasDefaultSchema("app");

            builder.Entity<ApplicationUser>().ToTable("Users", "auth");
            builder.Entity<IdentityRole>().ToTable("Roles", "auth");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "auth");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "auth");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "auth");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "auth");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "auth");

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}