using Hangfire;
using Saldoa.Application.Auth.Refresh;

namespace Saldoa.Worker
{
    internal class HangfireSchedulerWorker : BackgroundService
    {
        private readonly IRecurringJobManager _recurringJobManager;

        public HangfireSchedulerWorker(IRecurringJobManager recurringJobManager)
        {
            _recurringJobManager = recurringJobManager;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _recurringJobManager.AddOrUpdate<CleanExpiredRefreshTokensJob>(
                "CleanExpiredTokens",
                service => service.ExecuteAsync(CancellationToken.None),
                Cron.Daily);

            return Task.CompletedTask;
        }
    }
}
