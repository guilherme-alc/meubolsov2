using System.Text;
using FluentValidation;
using MeuBolso.API.Auth;
using MeuBolso.API.Endpoints.Auth;
using MeuBolso.API.Endpoints.Categories;
using MeuBolso.API.Identity;
using MeuBolso.API.Middlewares;
using MeuBolso.API.Persistence;
using MeuBolso.Application.Auth.Abstractions;
using MeuBolso.Application.Auth.Login;
using MeuBolso.Application.Auth.Logout;
using MeuBolso.Application.Auth.Refresh;
using MeuBolso.Application.Auth.Register;
using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Categories.Create;
using MeuBolso.Application.Identity.Abstractions;
using MeuBolso.Infrastructure.Categories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace MeuBolso.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "MeuBolso API",
                        Version = "v1",
                        Description = "MeuBolso API Documentation",
                        Contact = new OpenApiContact
                        {
                            Name = "Guilherme Campos",
                            Email = "guilhermealc01@gmail.com"
                        }
                    }
                ); 
                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", document)] = []
                });
            });
            
            // Habilita validacao de escopo para servicos
            if (builder.Environment.IsDevelopment())
            {
                builder.Host.UseDefaultServiceProvider(options =>
                {
                    options.ValidateScopes = true;
                });
            }
            
            builder.Services.AddDbContext<MeuBolsoDbContext>(opts =>
                opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            
            builder.Services.AddScoped<IJwtProvider, JwtProvider>();
            builder.Services.AddScoped<IIdentityService, IdentityService>();
            builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
            builder.Services.AddScoped<LoginUseCase>();
            builder.Services.AddScoped<RegisterUseCase>();
            builder.Services.AddScoped<RefreshUseCase>();
            builder.Services.AddScoped<LogoutUseCase>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<CreateCategoryUseCase>();

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
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwt.Secret)),
            
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            
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
            .AddDefaultTokenProviders();
            
            // Remove o cabecalho "Server" das respostas HTTP
            builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

            // Fallback policy para exigir autenticação em todas as rotas por padrao
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "docs/{documentName}/swagger.json";
                });

                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "docs";
                    c.SwaggerEndpoint("/docs/v1/swagger.json", "MeuBolso API v1");
                    c.EnablePersistAuthorization();
                });
            }
            
            app.UseHttpsRedirection();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            
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
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapAuthEndpoints();
            app.MapCategoriesEndpoint();

            app.Run();
        }
    }
}
