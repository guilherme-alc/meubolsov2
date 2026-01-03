namespace MeuBolso.Application.Auth.AuthDTO;

public record RegisterRequest(string Email, string Password, string? FullName);