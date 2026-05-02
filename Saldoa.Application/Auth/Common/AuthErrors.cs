using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Auth.Common
{
    public static class AuthErrors
    {
        public static Error Forbidden =>
            new("Auth.Forbidden", "Sem permissão", ErrorType.Forbidden);
        public static Error Unauthorized =>
            new("Auth.Unauthorized", $"", ErrorType.Unauthorized);
        public static Error InvalidAccess =>
            new("Auth.InvalidAccess", "Acesso Inválido", ErrorType.Unauthorized);
        public static Error AlreadyExists =>
            new("Auth.AlreadyExists", "Usuário já existe", ErrorType.Conflict);
    }
}
