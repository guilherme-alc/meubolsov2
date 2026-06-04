namespace Saldoa.Application.Auth.Abstractions;


public sealed record RefreshTokenResult(string RawToken, string TokenHash, DateTime ExpiresAt);

public interface IRefreshTokenGenerator
{
    RefreshTokenResult Generate();
}