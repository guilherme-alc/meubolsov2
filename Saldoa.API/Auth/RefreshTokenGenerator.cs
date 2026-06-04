using Saldoa.Application.Auth;
using Saldoa.Application.Auth.Abstractions;
using Microsoft.Extensions.Options;

namespace Saldoa.API.Auth;

public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    private readonly JwtOptions _options;
    public RefreshTokenGenerator(IOptions<JwtOptions> options) => _options = options.Value;

    public RefreshTokenResult Generate()
    {
        var raw = RefreshTokenCrypto.GenerateToken();
        var hash = RefreshTokenCrypto.HashToken(raw);
        var expiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenDays);
        return new RefreshTokenResult(raw, hash, expiresAt);
    }
}