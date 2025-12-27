using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Transactions.Abstractions;

namespace MeuBolso.Application.Common.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct = default);
}