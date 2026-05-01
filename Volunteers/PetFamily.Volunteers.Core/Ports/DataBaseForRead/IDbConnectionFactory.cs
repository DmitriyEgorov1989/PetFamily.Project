using System.Data;

namespace PetFamily.Core.Ports.DataBaseForRead;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}