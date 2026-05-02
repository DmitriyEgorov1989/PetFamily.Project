using System.Data;

namespace PetFamily.Volunteers.Core.Ports.DataBaseForRead;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}