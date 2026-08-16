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
        public static Error Invalid =>
            new("Auth.Invalid", "Dados informados inválidos", ErrorType.Validation);
        public static Error InvalidConfirmToken =>
            new("Auth.InvalidConfirmToken", "Token de confirmação inválido ou expirado.", ErrorType.Validation);
        public static Error EmailNotConfirmed =>
            new("Auth.EmailNotConfirmed", "Email não confirmado.", ErrorType.Validation);
        public static Error InvalidCredentials =>
            new("Auth.InvalidCredentials", "Credenciais inválidas.", ErrorType.Validation);
        public static Error UserNotFound =>
            new("Auth.UserNotFound", "Usuário não encontrado.", ErrorType.Validation);
        public static Error Unexpected =>
            new("Auth.Unexpected", "Falha inesperada.", ErrorType.Unexpected);
        public static Error InvalidResetToken =>
            new("Auth.InvalidResetToken", "Token de redefinição de senha inválido ou expirado.", ErrorType.Validation);
        public static Error InvalidPassword =>
            new("Auth.InvalidPassword", "A senha não atende aos requisitos de segurança.", ErrorType.Validation);
    }
}
