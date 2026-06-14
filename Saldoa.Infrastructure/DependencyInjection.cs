using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saldoa.Application.Auth.Abstractions;
using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Identity.Abstractions;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Infrastructure.Auth;
using Saldoa.Infrastructure.Identity;
using Saldoa.Infrastructure.Persistence;
using Saldoa.Infrastructure.Persistence.Repositories;

namespace Saldoa.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<SaldoaDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ICategoryBudgetRepository, CategoryBudgetRepository>();

            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<SaldoaDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
