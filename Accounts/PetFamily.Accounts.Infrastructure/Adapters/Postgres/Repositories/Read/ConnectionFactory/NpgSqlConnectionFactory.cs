using System.Data;
using Npgsql;
using PetFamily.Volunteers.Core.Ports.DataBaseForRead;

namespace PetFamily.Accounts.Infrastructure.Adapters.Postgres.Repositories.Read.ConnectionFactory;

public class NpgSqlConnectionFactory : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgSqlConnectionFactory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        return connection;
    }
}