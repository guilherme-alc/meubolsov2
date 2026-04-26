using Saldoa.API.Middlewares;
using Scalar.AspNetCore;

namespace Saldoa.API.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Configura o pipeline HTTP da API, centralizando middlewares,
    /// segurança, documentação e mapeamento de endpoints.
    /// </summary>
    public static WebApplication UseApi(this WebApplication app)
    {
        // Tratamento global de exceções
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        // HTTPS
        app.UseHttpsRedirection();
        
        // Security Headers
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
        
        // Scalar em dev
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi().AllowAnonymous();
            app.MapScalarApiReference("/docs", options =>
            {
                options.Title = "Saldoa API";
                options.Theme = ScalarTheme.Moon;
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                options.AddPreferredSecuritySchemes("Bearer")
                    .AddHttpAuthentication("Bearer", auth =>
                    {
                        auth.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
                    }).EnablePersistentAuthentication();
            }).AllowAnonymous();
        }

        // Segurança
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Endpoints
        app.MapEndpoints();
        
        return app;
    }
}