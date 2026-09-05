using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Saldoa.Application.Auth.ConfirmEmail;
using Saldoa.Application.Auth.Login;
using Saldoa.Application.Auth.Logout;
using Saldoa.Application.Auth.Refresh;
using Saldoa.Application.Auth.Register;
using Saldoa.Application.Auth.ResendConfirmEmail;
using Saldoa.Application.Auth.ResetPassword;
using Saldoa.Application.Auth.SendPasswordReset;
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
using Saldoa.Application.Transactions.ListByPeriod;
using Saldoa.Application.Transactions.Update;

namespace Saldoa.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

            services.AddScoped<LoginUseCase>();
            services.AddScoped<RegisterUseCase>();
            services.AddScoped<RefreshUseCase>();
            services.AddScoped<LogoutUseCase>();
            services.AddScoped<ConfirmEmailUseCase>();
            services.AddScoped<ResendConfirmEmailUseCase>();
            services.AddScoped<SendPasswordResetTokenUseCase>();
            services.AddScoped<ResetPasswordUseCase>();
            services.AddScoped<CreateCategoryUseCase>();
            services.AddScoped<UpdateCategoryUseCase>();
            services.AddScoped<GetCategoryByIdUseCase>();
            services.AddScoped<DeleteCategoryUseCase>();
            services.AddScoped<ListCategoriesUseCase>();
            services.AddScoped<TransactionBudgetAnalyzer>();
            services.AddScoped<CreateTransactionUseCase>();
            services.AddScoped<GetTransactionByIdUseCase>();
            services.AddScoped<UpdateTransactionUseCase>();
            services.AddScoped<DeleteTransactionUseCase>();
            services.AddScoped<ListTransactionsByPeriodUseCase>();
            services.AddScoped<GetInstallmentsByGroupIdUseCase>();
            services.AddScoped<CreateCategoryBudgetUseCase>();
            services.AddScoped<ListCategoryBudgetsUseCase>();
            services.AddScoped<DeleteCategoryBudgetUseCase>();
            services.AddScoped<GetCategoryBudgetByIdUseCase>();
            services.AddScoped<UpdateCategoryBudgetUseCase>();
            services.AddScoped<GetCategoryBudgetsByCategoryUseCase>();
            services.AddScoped<SendEmailConfirmationJob>();
            services.AddScoped<CleanExpiredRefreshTokensJob>();

            return services;
        }
    }
}
