using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Saldoa.API.OpenApi
{
    internal sealed class AuthOperationTransformer : IOpenApiOperationTransformer
    {
        private const string BearerSchemeId = "Bearer";

        public Task TransformAsync(
            OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            var metadata = context.Description.ActionDescriptor.EndpointMetadata;

            var hasAllowAnonymous = metadata.OfType<IAllowAnonymous>().Any();

            if (hasAllowAnonymous)
                return Task.CompletedTask;

            operation.Security ??= [];

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(BearerSchemeId, context.Document)] = []
            });

            return Task.CompletedTask;
        }
    }

}
