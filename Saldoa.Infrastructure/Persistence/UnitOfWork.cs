using Microsoft.EntityFrameworkCore;
using Npgsql;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Exceptions;

namespace Saldoa.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    readonly SaldoaDbContext _dbContext;

    public UnitOfWork(SaldoaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task SaveChangesAsync(
        CancellationToken ct = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new PersistenceConflictException(
                "Conflito de concorrência ao persistir dados.", ex);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            throw new PersistenceConflictException(
                "Já existe um registro com esses dados.", ex);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.ForeignKeyViolation
            })
        {
            throw new PersistenceConflictException(
                "Não foi possível concluir por vínculo com outro registro.", ex);
        }
    }
}