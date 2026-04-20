using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Core.Application.UseCases.ComonDto;
using PetFamily.Core.Ports.DataBaseForRead;
using Primitives;
using Serilog;
using System.Data;

namespace PetFamily.Infrastructure.Adapters.Postgres.ReadDatabase.Repository;

public class ReadRepository : IReadRepository
{
    private const string SQL =
        """
        SELECT 
            v.volunteer_id as VolunteerId,
            v.first_name as FirstName,
            v.middle_name as MiddleName,
            v.last_name as LastName,
            v.email,
            v.description,
            v.experience,
            v.phone_number as PhoneNumber,
            v.help_requisites as HelpRequisites,
            v.social_networks as SocialNetworks from volunteers as v
        where v.is_delete = false
        offset @OffSet rows
        fetch next @PageSize rows only;
        """;

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger _logger;

    public ReadRepository(IDbConnectionFactory connectionFactory, ILogger logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<VolunteerDto>, Error>> GetAllVolunteersWithPaginationAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@OffSet", (pageNumber - 1) * pageSize, DbType.Int32);
            parameters.Add("@PageSize", pageSize, DbType.Int32);

            var command = new CommandDefinition(SQL, parameters, cancellationToken: cancellationToken);

            var volunteers =
                await connection.QueryAsync<VolunteerDto>(command);

            if (volunteers is null || !volunteers.Any())
            {
                _logger.Information(
                    "No volunteers found for page {PageNumber} with page size {PageSize}",
                    pageNumber,
                    pageSize);
            }

            return Result.Success<IEnumerable<VolunteerDto>, Error>(volunteers);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error from Get Volunteers");
            return new Error("error.get.volunteers", ex.Message, ErrorType.InternalServerError);
        }
    }
}