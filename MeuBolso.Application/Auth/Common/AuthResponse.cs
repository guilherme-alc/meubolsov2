namespace MeuBolso.Application.Auth.Common;

public record AuthResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);