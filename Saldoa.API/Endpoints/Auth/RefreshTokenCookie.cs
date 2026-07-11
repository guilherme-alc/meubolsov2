namespace Saldoa.API.Endpoints.Auth;

internal static class RefreshTokenCookie
{
    public const string Name = "saldoa_refresh_token"; // nome do cookie
    private const string Path = "/auth"; // escopo de caminho do cookie para o navegador só enviar esse cookie para rotas que começam com /auth

    public static string? Read(HttpRequest request)
        => request.Cookies.TryGetValue(Name, out var refreshToken)
            ? refreshToken
            : null;

    public static void Append(HttpResponse response, string refreshToken, DateTime expiresAt)
        => response.Cookies.Append(Name, refreshToken, CreateOptions(expiresAt));

    public static void Delete(HttpResponse response)
        => response.Cookies.Delete(Name, CreateOptions(DateTimeOffset.UnixEpoch.UtcDateTime));

    private static CookieOptions CreateOptions(DateTime expiresAt)
        => new()
        {
            HttpOnly = true, // impede o js de ler o cookie
            Secure = true,
            SameSite = SameSiteMode.Strict, // bloqueia requisições de outros sites
            Expires = expiresAt,
            Path = Path
        };
}
