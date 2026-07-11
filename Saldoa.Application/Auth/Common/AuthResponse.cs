namespace Saldoa.Application.Auth.Common;

public sealed record AuthResult(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt)
{
    public AuthResponse ToResponse() // sempre retornar por aqui nos endpoints
        => new(AccessToken, AccessTokenExpiresAt, RefreshTokenExpiresAt);
}

public sealed record AuthResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);
