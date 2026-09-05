using Saldoa.Application.Auth.Abstractions;

namespace Saldoa.Application.Auth.Refresh
{
    public class CleanExpiredRefreshTokensJob
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public CleanExpiredRefreshTokensJob(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public Task ExecuteAsync(CancellationToken ct = default)
        {
            return _refreshTokenRepository.CleanExpiredTokensAsync(ct);
        }
    }
}
