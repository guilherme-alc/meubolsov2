namespace Saldoa.API.Endpoints.Auth;

internal static class AuthRequestSecurity
{
    private const string OriginHeaderName = "Origin";
    private const string SecFetchSiteHeaderName = "Sec-Fetch-Site";
    private const string CrossSite = "cross-site";
    private const string TrustedOriginsSection = "Auth:TrustedOrigins";

    public static bool IsTrustedRefreshRequest(HttpRequest request, IConfiguration configuration)
    {
        var secFetchSite = request.Headers[SecFetchSiteHeaderName].ToString();
        if (secFetchSite.Equals(CrossSite, StringComparison.OrdinalIgnoreCase))
            return false; // rejeita requisção de outro site pois pode ser tentativa de CSRF

        var origin = request.Headers[OriginHeaderName].ToString();
        if (string.IsNullOrWhiteSpace(origin))
            return true; // caso não exista header Origin, para dev local

        if (!Uri.TryCreate(origin, UriKind.Absolute, out var originUri))
            return false; // url inválida

        if (IsSameOrigin(originUri, request))
            return true; // caso front esteja na mesma origem

        var trustedOrigins = configuration
            .GetSection(TrustedOriginsSection) // origem diferente
            .Get<string[]>() ?? [];

        return trustedOrigins.Any(trustedOrigin =>
            Uri.TryCreate(trustedOrigin, UriKind.Absolute, out var trustedUri) &&
            HasSameOrigin(originUri, trustedUri));
    }

    public static IResult InvalidOriginProblem()
        => TypedResults.Problem(
            title: "Auth.InvalidOrigin",
            detail: "Origem da requisição não permitida.",
            statusCode: StatusCodes.Status403Forbidden);

    private static bool IsSameOrigin(Uri originUri, HttpRequest request)
    {
        var requestHost = request.Host.Host;
        var requestPort = request.Host.Port ?? GetDefaultPort(request.Scheme);

        return originUri.Scheme.Equals(request.Scheme, StringComparison.OrdinalIgnoreCase) &&
               originUri.Host.Equals(requestHost, StringComparison.OrdinalIgnoreCase) &&
               originUri.Port == requestPort;
    }

    private static bool HasSameOrigin(Uri first, Uri second)
        => first.Scheme.Equals(second.Scheme, StringComparison.OrdinalIgnoreCase) &&
           first.Host.Equals(second.Host, StringComparison.OrdinalIgnoreCase) &&
           first.Port == second.Port;

    private static int GetDefaultPort(string scheme)
        => scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            ? 443
            : 80;
}
