using Microsoft.AspNetCore.RateLimiting;
using Saldoa.API.Security;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace Saldoa.API.Extensions;

public static class RateLimitingExtensions
{
    public static WebApplicationBuilder AddApplicationRateLimiting(this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options => 
        {
            ConfigureRejection(options);
            ConfigureGlobalLimiter(options);
            ConfigureLoginPolicy(options);
            ConfigureRegisterPolicy(options);
            ConfigureRefreshPolicy(options);
            ConfigurePasswordRecoveryPolicy(options);
            ConfigureEmailConfirmationPolicy(options);
        });

        return builder;
    }

    private static void ConfigureRejection(RateLimiterOptions options)
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.OnRejected = async (context, cancellationToken) =>
        {
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
            }

            await context.HttpContext.Response.WriteAsJsonAsync(
                new
                {
                    title = "Muitas requisições",
                    detail = "Aguarde alguns instantes antes de tentar novamente.",
                    status = StatusCodes.Status429TooManyRequests
                },
                cancellationToken
            );
        };
    }

    private static void ConfigureGlobalLimiter(RateLimiterOptions options)
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var partitionKey = !string.IsNullOrWhiteSpace(userId) ? $"user:{userId}" : $"ip:{GetClientIp(httpContext)}";

            return RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey, _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 120,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 6,
                    QueueLimit = 0,
                    AutoReplenishment = true
                }
            );
        });
    }

    private static void ConfigureLoginPolicy(RateLimiterOptions options)
    {
        options.AddPolicy(RateLimitPolicies.Login, httpContext =>
        {
            var clientIp = GetClientIp(httpContext);

            return RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: $"login:{clientIp}",
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 6,
                    QueueLimit = 0,
                    AutoReplenishment = true
                }
            );
        });
    }

    private static void ConfigureRegisterPolicy(RateLimiterOptions options)
    {
        options.AddPolicy(RateLimitPolicies.Register, httpContext =>
        {
            var clientIp = GetClientIp(httpContext);

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: $"register:{clientIp}",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 3,
                    Window = TimeSpan.FromMinutes(10),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }
            );
        });
    }

    private static void ConfigureRefreshPolicy(RateLimiterOptions options)
    {
        options.AddPolicy(RateLimitPolicies.Refresh, httpContext =>
        {
            var clientIp = GetClientIp(httpContext);

            return RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: $"refresh:{clientIp}",
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 30,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 6,
                    QueueLimit = 0,
                    AutoReplenishment = true
                }
            );
        });
    }

    private static void ConfigurePasswordRecoveryPolicy(RateLimiterOptions options)
    {
        options.AddPolicy(RateLimitPolicies.PasswordRecovery, httpContext =>
        {
            var clientIp = GetClientIp(httpContext);

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: $"password-recovery:{clientIp}",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 3,
                    Window = TimeSpan.FromMinutes(15),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }
            );
        });
    }

    private static void ConfigureEmailConfirmationPolicy(RateLimiterOptions options)
    {
        options.AddPolicy(RateLimitPolicies.EmailConfirmation, httpContext =>
        {
            var clientIp = GetClientIp(httpContext);

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: $"email-confirmation:{clientIp}",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 3,
                    Window = TimeSpan.FromMinutes(15),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }
            );
        });
    }

    private static string GetClientIp(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}