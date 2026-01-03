namespace MeuBolso.Domain.Entities;

public class RefreshToken
{
    private RefreshToken() { }

    public RefreshToken(string token, string userId, DateTime expiresAt)
    {
        Token = token;
        UserId = userId;
        ExpiresAt = expiresAt;
    }
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Token { get; private set; } = string.Empty;
    public string UserId { get; private  set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public void Revoke() => IsRevoked = true;
}