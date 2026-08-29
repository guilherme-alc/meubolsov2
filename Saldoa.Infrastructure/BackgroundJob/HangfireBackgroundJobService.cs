using Hangfire;
using Saldoa.Application.Common.Abstractions;
using System.Linq.Expressions;

namespace Saldoa.Infrastructure.BackgroundJob
{
    public class HangfireBackgroundJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient _client;

        public HangfireBackgroundJobService(IBackgroundJobClient client)
        {
            _client = client;
        }

        public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
        {
            return _client.Enqueue(methodCall);
        }
    }
}
