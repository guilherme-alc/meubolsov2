using Saldoa.API.Extensions;

namespace Saldoa.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder
                .AddOpenApi()
                .AddInfrastructure()
                .AddAuth()
                .AddApplicationRateLimiting()
                .AddApplication();
            
            // Habilita validacao de escopo para servicos
            if (builder.Environment.IsDevelopment())
            {
                builder.Host.UseDefaultServiceProvider(options =>
                {
                    options.ValidateScopes = true;
                });
            }
            
            // Remove o cabecalho "Server" das respostas HTTP
            builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

            var app = builder.Build();

            app.UseApi();

            app.Run();
        }
    }
}
