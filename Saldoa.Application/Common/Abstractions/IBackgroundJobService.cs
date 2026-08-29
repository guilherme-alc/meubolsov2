using System.Linq.Expressions;

namespace Saldoa.Application.Common.Abstractions
{
    public interface IBackgroundJobService
    {
        string Enqueue<T>(Expression<Func<T, Task>> methodCall);
    }
}
