using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Saldoa.API.OpenApi;
using Saldoa.Application.Auth.Login;
using Saldoa.Application.Auth.Logout;
using Saldoa.Application.Auth.Refresh;
using Saldoa.Application.Auth.Register;
using Saldoa.Application.Categories.Create;
using Saldoa.Application.Categories.Delete;
using Saldoa.Application.Categories.GetById;
using Saldoa.Application.Categories.List;
using Saldoa.Application.Categories.Update;
using Saldoa.Application.CategoryBudgets.Create;
using Saldoa.Application.CategoryBudgets.Delete;
using Saldoa.Application.CategoryBudgets.GetCategoryBudgetByCategory;
using Saldoa.Application.CategoryBudgets.GetCategoryBudgetById;
using Saldoa.Application.CategoryBudgets.ListCategoryBudgets;
using Saldoa.Application.CategoryBudgets.Update;
using Saldoa.Application.Transactions.Common;
using Saldoa.Application.Transactions.Create;
using Saldoa.Application.Transactions.Delete;
using Saldoa.Application.Transactions.GetById;
using Saldoa.Application.Transactions.GetInstallmentsByGroupId;
using Saldoa.Application.Transactions.ListByMonth;
using Saldoa.Application.Transactions.ListByPeriod;
using Saldoa.Application.Transactions.Update;
using Saldoa.Infrastructure;
using Saldoa.Infrastructure.Auth;
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
    
    public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection(JwtOptions.SectionName));

        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddAuthenticatedUserFallbackPolicy();

        return builder;
    }

    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddInfrastructure(builder.Configuration);

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
        builder.Services.AddScoped<GetInstallmentsByGroupIdUseCase>();
        
        builder.Services.AddScoped<CreateCategoryBudgetUseCase>();
        builder.Services.AddScoped<ListCategoryBudgetsUseCase>();
        builder.Services.AddScoped<DeleteCategoryBudgetUseCase>();
        builder.Services.AddScoped<GetCategoryBudgetByIdUseCase>();
        builder.Services.AddScoped<UpdateCategoryBudgetUseCase>();
        builder.Services.AddScoped<GetCategoryBudgetsByCategoryUseCase>();
    
        return builder;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = configuration
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

        return services;
    }

    private static IServiceCollection AddAuthenticatedUserFallbackPolicy(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        return services;
    }
}
