using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Saldoa.API.OpenApi
{
    internal sealed class BearerSecuritySchemeTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider
    ) : IOpenApiDocumentTransformer
    {
        private const string BearerSchemeId = "Bearer";

        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var schemes = await authenticationSchemeProvider.GetAllSchemesAsync();

            if (!schemes.Any(s => s.Name == BearerSchemeId))
                return;

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

            document.Components.SecuritySchemes[BearerSchemeId] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Informe o token JWT no formato: Bearer {seu_token}"
            };
        }
    }
}
