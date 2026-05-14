using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.Accounts.Core.Ports;
using PetFamily.SharedKernel.Errors;
using PetFamily.Volunteers.Core.Ports.DataBaseForRead;
using Serilog;
using System.Data;

namespace PetFamily.Accounts.Infrastructure.Adapters.Postgres.Repositories.Read;

public class ReadRepository : IReadDataProvider
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger _logger;

    public ReadRepository(
        IDbConnectionFactory dbConnectionFactory,
        ILogger logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }

    public async Task<Result<List<string>, Error>> GetAllPermissionsByRoleIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        const string SQL =
            """
            SELECT p.code
            FROM accounts.permissions p
            JOIN accounts.role_permissions rp
                ON rp.permission_id = p.permission_id
            WHERE rp.role_id = @RoleId;
            """;
        try
        {
            using var connection =
                await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@RoleId", id, DbType.Guid);

            var command = new CommandDefinition(
                SQL,
                parameters,
                cancellationToken: cancellationToken);

            var permissions = (await connection.QueryAsync<string>(command)).ToList();

            if (!permissions.Any())
            {
                _logger.Warning("Permissions for role {RoleId} not found", id);
                return GeneralErrors.NotFound(nameof(id));
            }

            return permissions;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while getting permissions for role {RoleId}", id);
            return GeneralErrors.Failure(ex.Message);
        }
    }

    public async Task<Result<Role, Error>> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string SQL =
            """
            SELECT role_id AS Id, role_name AS Name
            FROM accounts.roles
            WHERE role_id = @RoleId;
            """;
        try
        {
            using var connection =
                await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@RoleId", id, DbType.Guid);

            var command = new CommandDefinition(
                SQL,
                parameters,
                cancellationToken: cancellationToken);

            var role = await connection.QueryFirstOrDefaultAsync<Role>(command);

            if (role == null)
            {
                _logger.Warning("Role with ID {RoleId} not found", id);
                return GeneralErrors.NotFound(nameof(id));
            }

            return role;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while getting role with ID {RoleId}", id);
            return GeneralErrors.Failure(e.Message);
        }
    }

    public async Task<Result<Role, Error>> GetRoleByNameAsync(
        string name, CancellationToken cancellationToken = default)
    {
        const string SQL =
            """
            SELECT id AS Id, name AS Name
            FROM accounts.roles
            WHERE name = @RoleName;
            """;
        try
        {
            using var connection =
                await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@RoleName", name, DbType.String);

            var command = new CommandDefinition(
                SQL,
                parameters,
                cancellationToken: cancellationToken);

            var role = await connection.QueryFirstOrDefaultAsync<Role>(command);

            if (role == null)
            {
                _logger.Warning("Role with name {RoleName} not found", name);
                return GeneralErrors.NotFound(nameof(name));
            }

            return role;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while getting role with name {RoleName}", name);
            return GeneralErrors.Failure(e.Message);
        }
    }
}