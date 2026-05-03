using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Core.Abstractions;
using System.Data;

namespace PetFamily.Accounts.Infrastructure.Adapters.Postgres;

public class UnitOfWork : IUnitOfWork
{
    private readonly AccountDbContext _dbContext;

    public UnitOfWork(AccountDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        return transaction.GetDbTransaction();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var trecker = _dbContext.ChangeTracker;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}