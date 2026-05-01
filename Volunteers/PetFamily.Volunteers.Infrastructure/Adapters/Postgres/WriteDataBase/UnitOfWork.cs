using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Core.Ports;

namespace PetFamily.Volunteers.Infrastructure.Adapters.Postgres.WriteDataBase;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
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