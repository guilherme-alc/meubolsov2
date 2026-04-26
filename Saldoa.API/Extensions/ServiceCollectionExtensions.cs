using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Saldoa.API.Auth;
using Saldoa.API.Identity;
using Saldoa.API.Infrastructure.Persistence;
using Saldoa.API.Infrastructure.Persistence.Repositories;
using Saldoa.API.OpenApi;
using Saldoa.Application.Auth.Abstractions;
using Saldoa.Application.Auth.Login;
using Saldoa.Application.Auth.Logout;
using Saldoa.Application.Auth.Refresh;
using Saldoa.Application.Auth.Register;
using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Categories.Create;
using Saldoa.Application.Categories.Delete;
using Saldoa.Application.Categories.GetById;
using Saldoa.Application.Categories.List;
using Saldoa.Application.Categories.Update;
using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.CategoryBudgets.Create;
using Saldoa.Application.CategoryBudgets.Delete;
using Saldoa.Application.CategoryBudgets.GetCategoryBudgetByCategory;
using Saldoa.Application.CategoryBudgets.GetCategoryBudgetById;
using Saldoa.Application.CategoryBudgets.ListCategoryBudgets;
using Saldoa.Application.CategoryBudgets.Update;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Identity.Abstractions;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;
using Saldoa.Application.Transactions.Create;
using Saldoa.Application.Transactions.Delete;
using Saldoa.Application.Transactions.GetById;
using Saldoa.Application.Transactions.ListByCategory;
using Saldoa.Application.Transactions.ListByMonth;
using Saldoa.Application.Transactions.ListByPeriod;
using Saldoa.Application.Transactions.Update;
using System.Text;

namespace Saldoa.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddOpenApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, ct) =>
            {
                document.Info.Title = "Saldoa API";
                document.Info.Version = "v1";
                document.Info.Description = "Saldoa API Documentation";

                return Task.CompletedTask;
            });
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddOperationTransformer<AuthOperationTransformer>();
        });

        return builder;
    }
    
    public static WebApplicationBuilder AddSecurity(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection(JwtOptions.SectionName));

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = builder.Configuration
                    .GetSection(JwtOptions.SectionName)
                    .Get<JwtOptions>()!;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return builder;
    }

    
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<SaldoaDbContext>(opts =>
            opts.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<SaldoaDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IJwtProvider, JwtProvider>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
        builder.Services.AddScoped<ICategoryBudgetRepository, CategoryBudgetRepository>();

        return builder;
    }

    
    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        builder.Services.AddScoped<LoginUseCase>();
        builder.Services.AddScoped<RegisterUseCase>();
        builder.Services.AddScoped<RefreshUseCase>();
        builder.Services.AddScoped<LogoutUseCase>();

        builder.Services.AddScoped<CreateCategoryUseCase>();
        builder.Services.AddScoped<UpdateCategoryUseCase>();
        builder.Services.AddScoped<GetCategoryByIdUseCase>();
        builder.Services.AddScoped<DeleteCategoryUseCase>();
        builder.Services.AddScoped<ListCategoriesUseCase>();

        builder.Services.AddScoped<TransactionBudgetAnalyzer>();
        builder.Services.AddScoped<CreateTransactionUseCase>();
        builder.Services.AddScoped<GetTransactionByIdUseCase>();
        builder.Services.AddScoped<UpdateTransactionUseCase>();
        builder.Services.AddScoped<DeleteTransactionUseCase>();
        builder.Services.AddScoped<ListTransactionsByPeriodUseCase>();
        builder.Services.AddScoped<ListTransactionsByMonthUseCase>();
        builder.Services.AddScoped<ListTransactionsByCategoryUseCase>();
        
        builder.Services.AddScoped<CreateCategoryBudgetUseCase>();
        builder.Services.AddScoped<ListCategoryBudgetsUseCase>();
        builder.Services.AddScoped<DeleteCategoryBudgetUseCase>();
        builder.Services.AddScoped<GetCategoryBudgetByIdUseCase>();
        builder.Services.AddScoped<UpdateCategoryBudgetUseCase>();
        builder.Services.AddScoped<GetCategoryBudgetsByCategoryUseCase>();
    
        return builder;
    }
}