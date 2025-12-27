using FluentValidation;
using MeuBolso.Application.Categories.Create;
using MeuBolso.Infrastructure.Identity;
using MeuBolso.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MeuBolso.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Habilita validacao de escopo para servicos
            builder.Host.UseDefaultServiceProvider(config =>
            {
                config.ValidateScopes = true;
            });
            
            builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
            
            builder.Services.AddDbContext<MeuBolsoDbContext>(opts =>
                opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>() // se usar roles
            .AddEntityFrameworkStores<MeuBolsoDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager()
            .AddApiEndpoints();

            // Remove o cabecalho "Server" das respostas HTTP
            builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

            // Fallback policy para exigir autenticacaoo em todas as rotas por padrao
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // Configuracao de cabecalhos de seguranca HTTP
            var policy = new HeaderPolicyCollection()
                .AddFrameOptionsDeny()
                .AddXssProtectionBlock()
                .AddContentTypeOptionsNoSniff()
                .AddReferrerPolicyStrictOriginWhenCrossOrigin()
                .AddCrossOriginOpenerPolicy(policyBuilder => policyBuilder.SameOrigin())
                .AddPermissionsPolicy(policy =>
                {
                    policy.AddCamera().None();
                    policy.AddMicrophone().None();
                    policy.AddGeolocation().None();
                });
            app.UseSecurityHeaders(policy);

            app.UseAuthorization();

            app.Run();
        }
    }
}
