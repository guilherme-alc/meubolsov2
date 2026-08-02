namespace Saldoa.Application.Auth.Register;

public record RegisterRequest(string Email, string Password, string FirstName, string? LastName);