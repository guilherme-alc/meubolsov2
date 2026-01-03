namespace MeuBolso.API.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;

    public int AccessTokenMinutes { get; init; }
    public int RefreshTokenDays { get; init; }
}