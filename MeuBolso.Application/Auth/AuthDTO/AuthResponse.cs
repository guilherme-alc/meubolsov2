namespace MeuBolso.Application.Auth.AuthDTO;

public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);